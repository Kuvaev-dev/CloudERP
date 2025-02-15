using Domain.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class UserSettingController : Controller
    {
        private readonly HttpClient _httpClient;

        public UserSettingController()
        {
            _httpClient = new HttpClient();
        }

        // GET: Create User
        public async Task<ActionResult> CreateUser(int? employeeID)
        {
            if (employeeID == null) return RedirectToAction("EP404", "EP");

            try
            {
                var response = await _httpClient.GetAsync($"user-setting/user/{employeeID}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении данных пользователя.";
                    return RedirectToAction("EP500", "EP");
                }

                var user = await response.Content.ReadAsAsync<User>();
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
                var response = await _httpClient.PostAsJsonAsync($"user-setting/create-user?companyId=123&branchId=456", user);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при создании пользователя.";
                    return RedirectToAction("EP500", "EP");
                }

                return RedirectToAction("Employees", "CompanyEmployee");
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
                var response = await _httpClient.GetAsync($"user-setting/user/{userID}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении данных пользователя.";
                    return RedirectToAction("EP500", "EP");
                }

                var user = await response.Content.ReadAsAsync<User>();
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
                var response = await _httpClient.PutAsJsonAsync($"user-setting/update-user", user);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при обновлении пользователя.";
                    return RedirectToAction("EP500", "EP");
                }

                return RedirectToAction("Employees", "CompanyEmployee");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при обновлении пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}
