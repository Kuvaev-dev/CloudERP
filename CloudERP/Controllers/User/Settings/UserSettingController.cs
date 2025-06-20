using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.User.Settings
{
    public class UserSettingController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;
        private readonly IPhoneNumberHelper _phoneNumberHelper;
        private readonly IPasswordHelper _passwordHelper;

        public UserSettingController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IPhoneNumberHelper phoneNumberHelper,
            IPasswordHelper passwordHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _phoneNumberHelper = phoneNumberHelper ?? throw new ArgumentNullException(nameof(phoneNumberHelper));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(phoneNumberHelper));
        }

        // GET: Create User
        public async Task<ActionResult> CreateUser(int? employeeID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (employeeID == null) return RedirectToAction("EP404", "EP");

            try
            {
                var employee = await _httpClient.GetAsync<Employee>($"companyemployeeapi/getbyid?id={employeeID}");
                await PopulateViewBag();

                return View(new Domain.Models.User 
                {
                    FullName = employee.FullName,
                    Email = employee.Email,
                    ContactNo = _phoneNumberHelper.ExtractNationalNumber(employee.ContactNumber),
                    UserName = employee.Email
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Create User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(Domain.Models.User user)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                user.Password = _passwordHelper.HashPassword(user.ContactNo, out string salt);
                user.Salt = salt;

                var success = await _httpClient.PostAsync(
                    $"usersettingapi/createuser?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}",
                    user);
                if (success)
                {
                    if (HttpContext.Session.GetInt32("UserTypeID") == 1) return RedirectToAction("Employees", "CompanyEmployee");
                    else return RedirectToAction("Employee", "BranchEmployee");
                }
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                await PopulateViewBag();
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Update User
        public async Task<ActionResult> UpdateUser(int? UserID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (UserID == null) return RedirectToAction("EP404", "EP");

            try
            {
                var user = await _httpClient.GetAsync<Domain.Models.User>($"usersettingapi/getuser?userId={UserID}");
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

        // POST: Update User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUser(Domain.Models.User user)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                user.Password = _passwordHelper.HashPassword(user.Password, out string salt);
                user.Salt = salt;

                var success = await _httpClient.PutAsync($"usersettingapi/updateuser", user);
                if (success)
                {
                    if (HttpContext.Session.GetInt32("UserTypeID") == 1) return RedirectToAction("Employees", "CompanyEmployee");
                    else return RedirectToAction("Employee", "BranchEmployee");
                }
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                await PopulateViewBag();
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            var userTypes = await _httpClient.GetAsync<List<UserType>>("usertypeapi/getall");
            ViewBag.UserTypeID = userTypes.Select(x => new SelectListItem
            {
                Value = x.UserTypeID.ToString(),
                Text = x.UserTypeName
            });
        }
    }
}
