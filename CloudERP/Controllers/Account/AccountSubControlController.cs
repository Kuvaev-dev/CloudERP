using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Account
{
    public class AccountSubControlController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public AccountSubControlController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
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
                var subControls = await _httpClient.GetAsync<IEnumerable<AccountSubControl>>(
                    $"accountsubcontrolapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(subControls);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                return View(new AccountSubControl());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSubControl model)
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

                if (model.AccountControlID > 0)
                {
                    var accountControl = await _httpClient.GetAsync<AccountControl>($"accountcontrolapi/getbyid?id={model.AccountControlID}");
                    if (accountControl != null)
                    {
                        model.AccountHeadID = accountControl.AccountHeadID;
                    }
                }

                if (ModelState.IsValid)
                {
                    await _httpClient.PostAsync("accountsubcontrolapi/create", model);
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

        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null) return RedirectToAction("Index");

                var accountControls = await _httpClient.GetAsync<List<AccountControl>>(
                    $"accountcontrolapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (accountControls == null) return RedirectToAction("EP404", "EP");

                var subControl = await _httpClient.GetAsync<AccountSubControl>($"accountsubcontrolapi/getbyid?id={id.Value}");
                if (subControl == null) return RedirectToAction("EP404", "EP");

                await PopulateViewBag();

                return View(new AccountSubControl());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSubControl model)
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

                if (model.AccountControlID > 0)
                {
                    var accountControl = await _httpClient.GetAsync<AccountControl>($"accountcontrolapi/getbyid?id={model.AccountControlID}");
                    if (accountControl != null)
                    {
                        model.AccountHeadID = accountControl.AccountHeadID;
                    }
                }

                if (ModelState.IsValid)
                {
                    await _httpClient.PutAsync($"accountsubcontrolapi/update?id={model.AccountSubControlID}", model);
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

        private async Task PopulateViewBag() =>
            ViewBag.AccountControlList = await _httpClient.GetAsync<List<AccountControl>>(
                    $"accountcontrolapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
    }
}