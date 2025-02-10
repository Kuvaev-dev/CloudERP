using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utils.Helpers;

namespace CloudERP.Controllers
{
    public class UserSettingController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly SessionHelper _sessionHelper;
        private readonly PasswordHelper _passwordHelper;

        public UserSettingController(
            IEmployeeRepository employeeRepository, 
            IUserRepository userRepository, 
            IUserTypeRepository userTypeRepository, 
            SessionHelper sessionHelper, 
            PasswordHelper passwordHelper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _userTypeRepository = userTypeRepository ?? throw new ArgumentNullException(nameof(IUserTypeRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(PasswordHelper));
        }

        // GET: CreateUser
        public async Task<ActionResult> CreateUser(int? employeeID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (employeeID == null) return RedirectToAction("EP404", "EP");

                var employee = await _employeeRepository.GetByIdAsync(employeeID.Value);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.EmployeeNotFound;
                    return RedirectToAction("EP500", "EP");
                }

                _sessionHelper.CompanyEmployeeID = employeeID;
                var hashedPassword = _passwordHelper.HashPassword(employee.ContactNumber, out string salt);

                var user = new User
                {
                    Email = employee.Email,
                    ContactNo = employee.ContactNumber,
                    FullName = employee.FullName,
                    IsActive = true,
                    Password = hashedPassword,
                    Salt = salt,
                    UserName = employee.Email
                };

                await PopulateViewBag();

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(User user)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateViewBag(user.UserTypeID);

                    return View(user);
                }

                var existingUser = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == user.Email && u.UserID != user.UserID);
                var existingEmployee = (await _employeeRepository.GetByBranchAsync(_sessionHelper.BranchID, _sessionHelper.CompanyID))
                    .FirstOrDefault(u => u.Email == user.Email && u.UserID == user.UserID);

                if (existingUser != null || existingEmployee != null)
                {
                    ViewBag.Message = Localization.CloudERP.Messages.Messages.EmailIsAlreadyRegistered;

                    await PopulateViewBag(user.UserTypeID);

                    return View(user);
                }

                user.Password = Request.Form["Password"];
                user.Salt = Request.Form["Salt"];

                await _userRepository.AddAsync(user);

                int? employeeID = _sessionHelper.CompanyEmployeeID;
                if (employeeID.HasValue)
                {
                    var employee = await _employeeRepository.GetByIdAsync(employeeID.Value);
                    if (employee != null)
                    {
                        employee.UserID = user.UserID;
                        await _employeeRepository.UpdateAsync(employee);
                    }
                    _sessionHelper.CompanyEmployeeID = null;
                }

                return RedirectToAction("Employees", "CompanyEmployee");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: UpdateUser
        public async Task<ActionResult> UpdateUser(int? userID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (userID == null) return RedirectToAction("EP404", "EP");

                var user = await _userRepository.GetByIdAsync(userID.Value);
                if (user == null)
                {
                    TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UserNotFound;
                    return RedirectToAction("EP500", "EP");
                }

                await PopulateViewBag(user.UserTypeID);

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUser(User user)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateViewBag(user.UserTypeID);
                    return View(user);
                }

                var existingUser = (await _userRepository.GetAllAsync()).FirstOrDefault(u => u.Email == user.Email && u.UserID != user.UserID);
                
                if (string.IsNullOrEmpty(user.Password))
                {
                    user.Password = Request.Form["Password"];
                    user.Salt = Request.Form["Salt"];
                }

                await _userRepository.UpdateAsync(user);
                await PopulateViewBag(user.UserTypeID);

                return RedirectToAction("Employees", "CompanyEmployee");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag(int? userTypeID = null)
        {
            var userTypes = await _userTypeRepository.GetAllAsync();
            ViewBag.UserTypeID = new SelectList(userTypes, "UserTypeID", "UserTypeName", userTypeID);
        }
    }
}