using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.User.Settings
{
    public class UserSettingController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public UserSettingController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: Create User
        public async Task<ActionResult> CreateUser(int? employeeID)
        {
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
                TempData["ErrorMessage"] = "Ошибка при получении данных пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Create User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(Domain.Models.User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                user.Password = Request.Form["Password"];
                user.Salt = Request.Form["Salt"];

                var success = await _httpClient.PostAsync(
                    $"usersettingapi/createuser?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}",
                    user);
                if (success) return RedirectToAction("Employees", "CompanyEmployee");

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при создании пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Update User
        public async Task<ActionResult> UpdateUser(int? userID)
        {
            if (userID == null) return RedirectToAction("EP404", "EP");

            try
            {
                var user = await _httpClient.GetAsync<Domain.Models.User>($"usersettingapi/getuser?userId={userID}");
                var userTypes = await _httpClient.GetAsync<List<UserType>>("usersettingapi/getusertypes");

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
                TempData["ErrorMessage"] = "Ошибка при получении данных пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Update User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateUser(Domain.Models.User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                user.Password = Request.Form["Password"];
                user.Salt = Request.Form["Salt"];

                var success = await _httpClient.PutAsync($"usersettingapi/updateuser", user);
                if (success) return RedirectToAction("Employees", "CompanyEmployee");

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при обновлении пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}
