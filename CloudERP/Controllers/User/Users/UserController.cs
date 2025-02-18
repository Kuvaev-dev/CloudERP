using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Utils.Helpers;

namespace CloudERP.Controllers
{
    public class UserController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;
        private readonly PasswordHelper _passwordHelper;

        public UserController(
            SessionHelper sessionHelper,
            PasswordHelper passwordHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
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
                var users = await _httpClient.GetAsync<List<User>>($"user");
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
                var user = await _httpClient.GetAsync<User>($"user/{id}");
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
                    model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                    model.Salt = salt;

                    var success = await _httpClient.PostAsync($"user/create", model);
                    if (success) return RedirectToAction("EP500", "EP");

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
                var user = await _httpClient.GetAsync<User>($"user/{id}");
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
                    if (string.IsNullOrEmpty(model.Password))
                    {
                        var existingUser = await _httpClient.GetAsync<User>($"user/{model.UserID}");

                        model.Password = existingUser.Password;
                        model.Salt = existingUser.Salt;
                    }
                    else
                    {
                        model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                        model.Salt = salt;
                    }

                    var response = await _httpClient.PutAsync($"user/update/{model.UserID}", model);

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