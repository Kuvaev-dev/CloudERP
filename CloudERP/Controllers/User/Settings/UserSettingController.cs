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

        public UserSettingController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IPhoneNumberHelper phoneNumberHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _phoneNumberHelper = phoneNumberHelper ?? throw new ArgumentNullException(nameof(phoneNumberHelper));
        }

        // GET: Create User
        public async Task<ActionResult> CreateUser(int? employeeID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (employeeID == null) return RedirectToAction("EP404", "EP");

            try
            {
                var user = await _httpClient.GetAsync<Domain.Models.User>($"usersettingapi/getuser?userId={employeeID}");
                var userTypes = await _httpClient.GetAsync<List<UserType>>("usersettingapi/getusertypes");

                ViewBag.UserTypeID = userTypes.Select(x => new SelectListItem
                {
                    Value = x.UserTypeID.ToString(),
                    Text = x.UserTypeName
                });

                return View(user);
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

            if (!ModelState.IsValid) return View(user);

            try
            {
                user.Password = Request.Form["Password"];
                user.Salt = Request.Form["Salt"];

                var success = await _httpClient.PostAsync(
                    $"usersettingapi/createuser?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}",
                    user);
                if (success) return RedirectToAction("Employees", "CompanyEmployee");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

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
                var userTypes = await _httpClient.GetAsync<IEnumerable<UserType>>("usertypeapi/getall");
                user.ContactNo = _phoneNumberHelper.ExtractNationalNumber(user.ContactNo);
                ViewBag.UserTypeID = userTypes.Select(x => new SelectListItem
                {
                    Value = x.UserTypeID.ToString(),
                    Text = x.UserTypeName,
                    Selected = x.UserTypeID == user.UserTypeID
                });

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

            if (!ModelState.IsValid) return View(user);

            try
            {
                user.Password = Request.Form["Password"];
                user.Salt = Request.Form["Salt"];

                var success = await _httpClient.PutAsync($"usersettingapi/updateuser", user);
                if (success) return RedirectToAction("Employees", "CompanyEmployee");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}
