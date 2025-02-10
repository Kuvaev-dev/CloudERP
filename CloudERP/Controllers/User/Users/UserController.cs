using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using Utils.Helpers;

namespace CloudERP.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly SessionHelper _sessionHelper;
        private readonly PasswordHelper _passwordHelper;

        public UserController(
            IUserRepository userRepository, 
            IUserTypeRepository userTypeRepository, 
            SessionHelper sessionHelper,
            PasswordHelper passwordHelper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _userTypeRepository = userTypeRepository ?? throw new ArgumentNullException(nameof(IUserTypeRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(PasswordHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var users = await _userRepository.GetAllAsync();
                if (users == null) return RedirectToAction("EP404", "EP");

                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SubBranchUser()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var users = await _userRepository.GetByBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchTypeID, _sessionHelper.BranchID);
                if (users == null) return RedirectToAction("EP404", "EP");

                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return RedirectToAction("EP404", "EP");

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                return View(new User());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                    model.Salt = salt;

                    await _userRepository.AddAsync(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return RedirectToAction("EP404", "EP");

                var userTypes = await _userTypeRepository.GetAllAsync();
                if (userTypes == null) return RedirectToAction("EP404", "EP");

                await PopulateViewBag(user.UserTypeID);

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        var existingUser = await _userRepository.GetByIdAsync(model.UserID);
                        model.Password = existingUser.Password;
                        model.Salt = existingUser.Salt;
                    }
                    else
                    {
                        model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                        model.Salt = salt;
                    }

                    await _userRepository.UpdateAsync(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag(int? userTypeID = null)
        {
            ViewBag.UserTypeID = new SelectList(await _userTypeRepository.GetAllAsync(), "UserTypeID", "UserType", userTypeID);
        }
    }
}