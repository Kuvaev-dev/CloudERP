using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Account
{
    public class AccountHeadController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public AccountHeadController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var accountHeads = await _httpClient.GetAsync<IEnumerable<AccountHead>>("accountheadapi/getall");
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

                var success = await _httpClient.PostAsync("accountheadapi/create", model);
                if (success) return RedirectToAction("Index");

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
                var accountHead = await _httpClient.GetAsync<AccountHead>($"accountheadapi/getbyid?id={id}");
                if (accountHead == null) return NotFound();

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

                var success = await _httpClient.PutAsync($"accountheadapi/update?id={model.AccountHeadID}", model);
                if (success) return RedirectToAction("Index");

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