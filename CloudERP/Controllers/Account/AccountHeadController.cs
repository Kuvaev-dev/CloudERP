using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using API.Helpers;
using CloudERP.Helpers;
using Domain.Models;

namespace CloudERP.Controllers
{
    public class AccountHeadController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public AccountHeadController(SessionHelper sessionHelper)
        {
            _httpClient = new HttpClientHelper();
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var accountHeads = await _httpClient.GetAsync<List<AccountHead>>("");
                return View(accountHeads);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new AccountHead());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountHead model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.UserID = _sessionHelper.UserID;
                var success = await _httpClient.PostAsync("", model);

                if (success)
                    return RedirectToAction("Index");

                TempData["ErrorMessage"] = "Error Creating a New Record";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var accountHead = await _httpClient.GetAsync<AccountHead>(id.ToString());
                if (accountHead == null) return HttpNotFound();

                return View(accountHead);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountHead model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.UserID = _sessionHelper.UserID;
                var success = await _httpClient.PutAsync(model.AccountHeadID.ToString(), model);

                if (success)
                    return RedirectToAction("Index");

                TempData["ErrorMessage"] = "Error Updating a Record";
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("Error", "Home");
            }
        }
    }
}