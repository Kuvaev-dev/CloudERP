using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using System.Collections.Generic;

namespace CloudERP.Controllers
{
    public class AccountSettingController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public AccountSettingController(
            SessionHelper sessionHelper)
        {
            _httpClient = new HttpClientHelper();
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var accountSettings = await _httpClient.GetAsync<List<AccountSetting>>(
                    $"account-control?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (accountSettings == null) return RedirectToAction("EP404", "EP");

                return View(accountSettings);
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
                await PopulateDropdownsAsync();
                return View(new AccountSetting());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSetting model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;
                model.IsGlobal = false;

                if (ModelState.IsValid)
                {
                    await _httpClient.PostAsync("account-setting/create", model);
                    return RedirectToAction("Index");
                }

                await PopulateDropdownsAsync();

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

                var accountSetting = await _httpClient.GetAsync<AccountSetting>($"account-setting/{id.Value}");
                if (accountSetting == null) return RedirectToAction("EP404", "EP");

                await PopulateDropdownsAsync();

                return View(accountSetting);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSetting model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _httpClient.PutAsync($"account-setting/update/{model.AccountSettingID}", model);
                    return RedirectToAction("Index");
                }

                await PopulateDropdownsAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task<SelectList> CreateSelectListAsync<T>(
            Func<Task<IEnumerable<T>>> getDataFunc,
            string dataValueField,
            string dataTextField,
            object selectedValue = null)
        {
            var data = await getDataFunc();
            return new SelectList(data, dataValueField, dataTextField, selectedValue);
        }

        private async Task PopulateDropdownsAsync(AccountSetting model = null)
        {
            ViewBag.AccountHeadList = await CreateSelectListAsync<AccountHead>(
                async () => await _httpClient.GetAsync<List<AccountHead>>("account-head"),
                "AccountHeadID",
                "AccountHeadName",
                model?.AccountHeadID);

            ViewBag.AccountControlList = await CreateSelectListAsync<AccountControl>(
                async () => await _httpClient.GetAsync<List<AccountControl>>(
                    $"account-control?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}"),
                "AccountControlID",
                "AccountControlName",
                model?.AccountControlID);

            ViewBag.AccountSubControlList = await CreateSelectListAsync<AccountSubControl>(
                async () => await _httpClient.GetAsync<List<AccountSubControl>>(
                    $"account-sub-control?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}"),
                "AccountSubControlID",
                "AccountSubControlName",
                model?.AccountSubControlID);

            ViewBag.AccountActivityList = await CreateSelectListAsync<AccountActivity>(
                async () => await _httpClient.GetAsync<List<AccountActivity>>("account-activity"),
                "AccountActivityID",
                "Name",
                model?.AccountActivityID);
        }
    }
}