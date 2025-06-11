using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.Account
{
    public class AccountSettingController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public AccountSettingController(
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
                var accountSettings = await _httpClient.GetAsync<IEnumerable<AccountSetting>>(
                    $"accountsettingapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(accountSettings);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateDropdownsAsync();
                return View(new AccountSetting());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSetting model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;
                model.IsGlobal = false;

                var success = await _httpClient.PostAsync("accountsettingapi/create", model);
                if (success) return RedirectToAction("Index");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                await PopulateDropdownsAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

                var accountSetting = await _httpClient.GetAsync<AccountSetting>($"accountsettingapi/getbyid?id={id.Value}");
                if (accountSetting == null) return RedirectToAction("EP404", "EP");

                await PopulateDropdownsAsync();

                return View(accountSetting);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSetting model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid) return View(model);

            try
            {
                var success = await _httpClient.PutAsync($"accountsettingapi/update?id={model.AccountSettingID}", model);
                if (success) return RedirectToAction("Index");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                await PopulateDropdownsAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task<SelectList> CreateSelectListAsync<T>(
            Func<Task<IEnumerable<T>>> getDataFunc,
            string dataValueField,
            string dataTextField,
            object? selectedValue = null)
        {
            var data = await getDataFunc();
            return new SelectList(data, dataValueField, dataTextField, selectedValue);
        }

        private async Task PopulateDropdownsAsync(AccountSetting? model = null)
        {
            ViewBag.AccountHeadList = await CreateSelectListAsync<AccountHead>(
                async () => await _httpClient.GetAsync<List<AccountHead>>("accountheadapi/getall") ?? [],
                "AccountHeadID",
                "AccountHeadName",
                model?.AccountHeadID);

            ViewBag.AccountControlList = await CreateSelectListAsync<AccountControl>(
                async () => await _httpClient.GetAsync<List<AccountControl>>(
                    $"accountcontrolapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}") ?? [],
                "AccountControlID",
                "AccountControlName",
                model?.AccountControlID);

            ViewBag.AccountSubControlList = await CreateSelectListAsync<AccountSubControl>(
                async () => await _httpClient.GetAsync<List<AccountSubControl>>(
                    $"accountsubcontrolapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}") ?? [],
                "AccountSubControlID",
                "AccountSubControlName",
                model?.AccountSubControlID);

            ViewBag.AccountActivityList = await CreateSelectListAsync<AccountActivity>(
                async () => await _httpClient.GetAsync<List<AccountActivity>>("accountactivityapi/getall") ?? [],
                "AccountActivityID",
                "Name",
                model?.AccountActivityID);
        }
    }
}