using Domain.Models;
using Domain.UtilsAccess;
using Localization.CloudERP.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.User.Users
{
    public class UserController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;
        private readonly IPhoneNumberHelper _phoneNumberHelper;
        private readonly IPasswordHelper _passwordHelper;

        public UserController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IPhoneNumberHelper phoneNumberHelper,
            IPasswordHelper passwordHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _phoneNumberHelper = phoneNumberHelper ?? throw new ArgumentNullException(nameof(phoneNumberHelper));
            _passwordHelper = passwordHelper;
        }

        // GET: User
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var users = await _httpClient.GetAsync<List<Domain.Models.User>>($"userapi/getall");
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User
        public async Task<ActionResult> SubBranchUser()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var users = await _httpClient.GetAsync<List<Domain.Models.User>>(
                    $"userapi/getbybranch?companyId={_sessionHelper.CompanyID}&branchTypeId={_sessionHelper.BranchTypeID}&branchId={_sessionHelper.BranchID}");
                return View(users ?? []);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Details/{id}
        public async Task<ActionResult> Details(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var user = await _httpClient.GetAsync<Domain.Models.User>($"userapi/getbyid?id={id}");
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Create
        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            await PopulateViewBag();

            return View(new Domain.Models.User());
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Domain.Models.User model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid) return View(model);

            try
            {   
                var success = await _httpClient.PostAsync($"user/create", model);
                if (success) return RedirectToAction("Index"); 
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Edit/{id}
        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var user = await _httpClient.GetAsync<Domain.Models.User>($"userapi/getbyid?id={id}");
                user.ContactNo = _phoneNumberHelper.ExtractNationalNumber(user.ContactNo);
                await PopulateViewBag();

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Domain.Models.User model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var newPassword = Request.Form["NewPassword"].ToString();

            await PopulateViewBag();

            if (!string.IsNullOrWhiteSpace(newPassword) && newPassword.Length < 6)
            {
                ModelState.AddModelError("Password", Localization.Domain.Localization.StringLengthMinMaxValidation);
            }

            if (!ModelState.IsValid) return View(model);

            try
            {
                var existingUser = await _httpClient.GetAsync<Domain.Models.User>($"userapi/getbyid?id={model.UserID}");

                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    model.Password = existingUser.Password;
                    model.Salt = existingUser.Salt;
                }
                else
                {
                    model.Password = _passwordHelper.HashPassword(newPassword, out string salt);
                    model.Salt = salt;
                }

                var success = await _httpClient.PutAsync($"userapi/update", model);
                if (success) return RedirectToAction("Index");

                ViewBag.ErrorMessage = Messages.AlreadyExists;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            var userTypes = await _httpClient.GetAsync<IEnumerable<UserType>>($"usertypeapi/getall");
            ViewBag.UserTypeList = userTypes?.Select(ut => new SelectListItem
            {
                Value = ut.UserTypeID.ToString(),
                Text = ut.UserTypeName
            });
        }
    }
}