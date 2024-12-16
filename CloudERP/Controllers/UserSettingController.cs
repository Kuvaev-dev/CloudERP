using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class UserSettingController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IUserService _userService;
        private readonly IUserTypeService _userTypeService;
        private readonly IMapper<User, UserMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public UserSettingController(IEmployeeService employeeService, IUserService userService, IUserTypeService userTypeService, IMapper<User, UserMV> mapper, SessionHelper sessionHelper)
        {
            _employeeService = employeeService;
            _userService = userService;
            _userTypeService = userTypeService;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        // GET: CreateUser
        public async Task<ActionResult> CreateUser(int? employeeID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (employeeID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var employee = await _employeeService.GetByIdAsync(employeeID.Value);
            if (employee == null)
            {
                TempData["ErrorMessage"] = Resources.Messages.EmployeeNotFound;
                return RedirectToAction("EP500", "EP");
            }

            _sessionHelper.CompanyEmployeeID = employeeID;
            var hashedPassword = PasswordHelper.HashPassword(employee.ContactNumber, out string salt);

            var user = new UserMV
            {
                Email = employee.Email,
                ContactNo = employee.ContactNumber,
                FullName = employee.FullName,
                IsActive = true,
                Password = hashedPassword,
                Salt = salt,
                UserName = employee.Email
            };

            var userTypes = await _userTypeService.GetAllAsync();
            ViewBag.UserTypeID = new SelectList(userTypes, "UserTypeID", "UserType");

            return View(_mapper.MapToDomain(user));
        }

        // POST: CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(UserMV userViewModel)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                var userTypes = await _userTypeService.GetAllAsync();
                ViewBag.UserTypeID = new SelectList(userTypes, "UserTypeID", "UserType", userViewModel.UserTypeID);
                return View(userViewModel);
            }

            var existingUser = (await _userService.GetAllAsync()).FirstOrDefault(u => u.Email == userViewModel.Email && u.UserID != userViewModel.UserID);
            if (existingUser != null)
            {
                ViewBag.Message = Resources.Messages.EmailIsAlreadyRegistered;

                var userTypes = await _userTypeService.GetAllAsync();
                ViewBag.UserTypeID = new SelectList(userTypes, "UserTypeID", "UserType", userViewModel.UserTypeID);

                return View(userViewModel);
            }

            userViewModel.Password = Request.Form["Password"];
            userViewModel.Salt = Request.Form["Salt"];

            var domainUser = _mapper.MapToDomain(userViewModel);
            await _userService.CreateAsync(domainUser);

            int? employeeID = _sessionHelper.CompanyEmployeeID;
            if (employeeID.HasValue)
            {
                var employee = await _employeeService.GetByIdAsync(employeeID.Value);
                if (employee != null)
                {
                    employee.UserID = domainUser.UserID;
                    await _employeeService.UpdateAsync(employee);
                }
                _sessionHelper.CompanyEmployeeID = null;
            }

            return RedirectToAction("Index", "User");
        }

        // GET: UpdateUser
        public async Task<ActionResult> UpdateUser(int? userID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (userID == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var user = await _userService.GetByIdAsync(userID.Value);
            if (user == null)
            {
                TempData["ErrorMessage"] = Resources.Messages.UserNotFound;
                return RedirectToAction("EP500", "EP");
            }

            var viewModel = _mapper.MapToViewModel(user);
            var userTypes = await _userTypeService.GetAllAsync();
            ViewBag.UserTypeID = new SelectList(userTypes, "UserTypeID", "UserType", user.UserTypeID);

            return View(viewModel);
        }

        // POST: UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUser(UserMV userViewModel)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                var userTypes = await _userTypeService.GetAllAsync();
                ViewBag.UserTypeID = new SelectList(userTypes, "UserTypeID", "UserType", userViewModel.UserTypeID);
                return View(userViewModel);
            }

            var existingUser = (await _userService.GetAllAsync()).FirstOrDefault(u => u.Email == userViewModel.Email && u.UserID != userViewModel.UserID);
            if (existingUser != null)
            {
                ViewBag.Message = Resources.Messages.EmailIsAlreadyRegistered;

                var userTypes = await _userTypeService.GetAllAsync();
                ViewBag.UserTypeID = new SelectList(userTypes, "UserTypeID", "UserType", userViewModel.UserTypeID);

                return View(userViewModel);
            }

            var domainUser = _mapper.MapToDomain(userViewModel);
            await _userService.UpdateAsync(domainUser);

            return RedirectToAction("Index", "User");
        }
    }
}