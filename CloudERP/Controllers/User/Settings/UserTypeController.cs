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
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public UserTypeController(SessionHelper sessionHelper)
        {
            _httpClient = new HttpClientHelper();
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: Index
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var userTypes = await _httpClient.GetAsync<List<UserType>>("user-type");
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
                var userType = await _httpClient.GetAsync<UserType>($"user-type/{id}");
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
                    var success = await _httpClient.PostAsync("user-type/create", model);
                    if (success) return RedirectToAction("Index");
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
                var userType = await _httpClient.GetAsync<UserType>($"user-type/{id}");
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
                    var success = await _httpClient.PutAsync($"user-type/update/{model.UserTypeID}", model);
                    if (success) return RedirectToAction("Index");
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