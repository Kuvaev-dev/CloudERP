using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using CloudERP.Facades;

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
                await PopulateDropdowns();

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

                if (ModelState.IsValid)
                {
                    await _accountSettingFacade.AccountSettingRepository.AddAsync(model);
                    return RedirectToAction("Index");
                }

                await PopulateDropdowns();

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
                if (setting == null) return HttpNotFound();

                await PopulateDropdownsWithModel(setting);

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
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;

                if (ModelState.IsValid)
                {
                    await _accountSettingFacade.AccountSettingRepository.UpdateAsync(model);
                    return RedirectToAction("Index");
                }

                await PopulateDropdownsWithModel(model);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateDropdowns()
        {
            ViewBag.AccountHeadList = new SelectList(
                await _accountSettingFacade.AccountHeadRepository.GetAllAsync(), 
                "AccountHeadID", 
                "AccountHeadName");
            ViewBag.AccountControlList = new SelectList(
                await _accountSettingFacade.AccountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID), 
                "AccountControlID", 
                "AccountControlName");
            ViewBag.AccountSubControlList = new SelectList(
                await _accountSettingFacade.AccountSubControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID), 
                "AccountSubControlID", 
                "AccountSubControlName");
            ViewBag.AccountActivityList = new SelectList(
                await _accountSettingFacade.AccountActivityRepository.GetAllAsync(), 
                "AccountActivityID", 
                "Name");
        }

        private async Task PopulateDropdownsWithModel(AccountSetting model)
        {
            ViewBag.AccountHeadList = new SelectList(
                await _accountSettingFacade.AccountHeadRepository.GetAllAsync(),
                "AccountHeadID",
                "AccountHeadName", 
                model.AccountHeadID);
            ViewBag.AccountControlList = new SelectList(
                await _accountSettingFacade.AccountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID),
                "AccountControlID",
                "AccountControlName",
                model.AccountControlID);
            ViewBag.AccountSubControlList = new SelectList(
                await _accountSettingFacade.AccountSubControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID),
                "AccountSubControlID",
                "AccountSubControlName",
                model.AccountSubControlID);
            ViewBag.AccountActivityList = new SelectList(
                await _accountSettingFacade.AccountActivityRepository.GetAllAsync(),
                "AccountActivityID",
                "Name",
                model.AccountActivityID);
        }
    }
}