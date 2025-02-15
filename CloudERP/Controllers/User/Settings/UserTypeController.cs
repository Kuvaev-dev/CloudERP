using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;

namespace CloudERP.Controllers
{
    public class UserTypeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly SessionHelper _sessionHelper;
        private readonly string _apiBaseUrl;

        public UserTypeController()
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "/api/user-types";
        }

        // GET: Index
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении типов пользователей.";
                    return RedirectToAction("EP500", "EP");
                }

                var userTypes = await response.Content.ReadAsAsync<IEnumerable<UserType>>();
                return View(userTypes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при получении типов пользователей: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении информации о типе пользователя.";
                    return RedirectToAction("EP500", "EP");
                }

                var userType = await response.Content.ReadAsAsync<UserType>();
                return View(userType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при получении информации о типе пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new UserType());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserType model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, model);
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Ошибка при создании типа пользователя.";
                        return RedirectToAction("EP500", "EP");
                    }

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при создании типа пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении информации о типе пользователя.";
                    return RedirectToAction("EP500", "EP");
                }

                var userType = await response.Content.ReadAsAsync<UserType>();
                return View(userType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при получении информации о типе пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserType model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{model.UserTypeID}", model);
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Ошибка при обновлении типа пользователя.";
                        return RedirectToAction("EP500", "EP");
                    }

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при обновлении типа пользователя: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}