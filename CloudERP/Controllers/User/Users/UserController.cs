using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Utils.Helpers;

namespace CloudERP.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://yourapiurl.com/api/user";

        private readonly SessionHelper _sessionHelper;
        private readonly PasswordHelper _passwordHelper;

        public UserController(
            SessionHelper sessionHelper,
            PasswordHelper passwordHelper)
        {
            _httpClient = new HttpClient();
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(PasswordHelper));
        }

        // GET: User
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/all");
                if (!response.IsSuccessStatusCode) return RedirectToAction("EP500", "EP");

                var users = await response.Content.ReadAsAsync<IEnumerable<User>>();
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode) return RedirectToAction("EP404", "EP");

                var user = await response.Content.ReadAsAsync<User>();
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new User());
        }

        // POST: User/Create
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
                    // Хешируем пароль
                    model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                    model.Salt = salt;

                    var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}/create", model);
                    if (!response.IsSuccessStatusCode) return RedirectToAction("EP500", "EP");

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode) return RedirectToAction("EP404", "EP");

                var user = await response.Content.ReadAsAsync<User>();
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: User/Edit
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
                    // Если пароль не был изменён, сохраняем старый
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{model.UserID}");
                        if (!response.IsSuccessStatusCode) return RedirectToAction("EP500", "EP");

                        var existingUser = await response.Content.ReadAsAsync<User>();
                        model.Password = existingUser.Password;
                        model.Salt = existingUser.Salt;
                    }
                    else
                    {
                        // Хешируем новый пароль
                        model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                        model.Salt = salt;
                    }

                    var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/update/{model.UserID}", model);
                    if (!response.IsSuccessStatusCode) return RedirectToAction("EP500", "EP");

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}