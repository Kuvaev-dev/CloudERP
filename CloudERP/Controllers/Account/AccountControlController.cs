using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Account
{
    public class AccountControlController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public AccountControlController(
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
                var accountControls = await _httpClient.GetAsync<IEnumerable<AccountControl>>(
                    $"accountcontrolapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (accountControls == null) return RedirectToAction("EP404", "EP");

                return View(accountControls);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();
                return View(new AccountControl());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountControl model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.BranchID = _sessionHelper.BranchID;
                model.CompanyID = _sessionHelper.CompanyID;
                model.UserID = _sessionHelper.UserID;
                model.IsGlobal = false;

                await PopulateViewBag();

                if (ModelState.IsValid)
                {
                    await _httpClient.PostAsync("accountcontrolapi/create", model);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null) return RedirectToAction("Index");

                var accountControl = await _httpClient.GetAsync<AccountControl>($"accountcontrolapi/getbyid?id={id.Value}");
                if (accountControl == null) return RedirectToAction("EP404", "EP");

                await PopulateViewBag();

                return View(accountControl);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountControl model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _httpClient.PutAsync($"accountcontrolapi/update?id={model.AccountControlID}", model);
                    return RedirectToAction("Index");
                }

                await PopulateViewBag();
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag() =>
            ViewBag.AccountHeadList = await _httpClient.GetAsync<List<AccountHead>>("accountheadapi/getall");
    }
}