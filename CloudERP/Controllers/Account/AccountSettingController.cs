using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using CloudERP.Facades;
using System.Collections.Generic;

namespace CloudERP.Controllers
{
    public class AccountSettingController : Controller
    {
        private readonly AccountSettingFacade _accountSettingFacade;
        private readonly SessionHelper _sessionHelper;

        public AccountSettingController(
            AccountSettingFacade accountSettingFacade,
            SessionHelper sessionHelper)
        {
            _accountSettingFacade = accountSettingFacade ?? throw new ArgumentNullException(nameof(AccountSettingFacade));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var settings = await _accountSettingFacade.AccountSettingRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                if (settings == null) return RedirectToAction("EP404", "EP");

                return View(settings);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                    await _accountSettingFacade.AccountSettingRepository.AddAsync(model);
                    return RedirectToAction("Index");
                }

                await PopulateDropdownsAsync();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var setting = await _accountSettingFacade.AccountSettingRepository.GetByIdAsync(id);
                if (setting == null) return RedirectToAction("EP404", "EP");

                await PopulateDropdownsAsync(setting);

                return View(setting);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                    await _accountSettingFacade.AccountSettingRepository.UpdateAsync(model);
                    return RedirectToAction("Index");
                }

                await PopulateDropdownsAsync(model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
            ViewBag.AccountHeadList = await CreateSelectListAsync(
                _accountSettingFacade.AccountHeadRepository.GetAllAsync,
                "AccountHeadID",
                "AccountHeadName",
                model?.AccountHeadID);

            ViewBag.AccountControlList = await CreateSelectListAsync(
                () => _accountSettingFacade.AccountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID),
                "AccountControlID",
                "AccountControlName",
                model?.AccountControlID);

            ViewBag.AccountSubControlList = await CreateSelectListAsync(
                () => _accountSettingFacade.AccountSubControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID),
                "AccountSubControlID",
                "AccountSubControlName",
                model?.AccountSubControlID);

            ViewBag.AccountActivityList = await CreateSelectListAsync(
                _accountSettingFacade.AccountActivityRepository.GetAllAsync,
                "AccountActivityID",
                "Name",
                model?.AccountActivityID);
        }
    }
}