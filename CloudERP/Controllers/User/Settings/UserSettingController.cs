using CloudERP.Helpers;
using Domain.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class UserSettingController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public UserSettingController(SessionHelper sessionHelper)
        {
            _httpClient = new HttpClientHelper();
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: Create User
        public async Task<ActionResult> CreateUser(int? employeeID)
        {
            if (employeeID == null) return RedirectToAction("EP404", "EP");

            try
            {
                var user = await _httpClient.GetAsync<User>($"user-setting/user/{employeeID}");
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
        public async Task<ActionResult> CreateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                user.Password = Request.Form["Password"];
                user.Salt = Request.Form["Salt"];

                var success = await _httpClient.PostAsync($"user-setting/create-user?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}", user);
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
                var user = await _httpClient.GetAsync<User>($"user-setting/user/{userID}");
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
        public async Task<ActionResult> UpdateUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            try
            {
                user.Password = Request.Form["Password"];
                user.Salt = Request.Form["Salt"];

                var success = await _httpClient.PutAsync($"user-setting/update-user", user);
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
