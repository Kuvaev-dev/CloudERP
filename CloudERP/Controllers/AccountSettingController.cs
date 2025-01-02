using System.Linq;
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
                if (ModelState.IsValid)
                {
                    model.CompanyID = _sessionHelper.CompanyID;
                    model.BranchID = _sessionHelper.BranchID;

                    await _accountSettingFacade.AccountSettingRepository.AddAsync(model);

                    TempData["SuccessMessage"] = "Account Setting successfully created.";
                    return RedirectToAction("Index");
                }

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
                if (setting == null)
                    return HttpNotFound();

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
                    model.CompanyID = _sessionHelper.CompanyID;
                    model.BranchID = _sessionHelper.BranchID;

                    await _accountSettingFacade.AccountSettingRepository.UpdateAsync(model);

                    TempData["SuccessMessage"] = "Account Setting successfully updated.";
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetAccountControls(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var controls = await _accountSettingFacade.AccountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                var filteredControls = controls.Where(c => c.AccountHeadID == id)
                                               .Select(c => new
                                               {
                                                   c.AccountControlID,
                                                   c.AccountControlName
                                               });

                return Json(new { data = filteredControls }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public async Task<ActionResult> GetSubControls(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var subControls = await _accountSettingFacade.AccountSubControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                var filteredSubControls = subControls.Where(s => s.AccountControlID == id)
                                                     .Select(s => new
                                                     {
                                                         s.AccountSubControlID,
                                                         s.AccountSubControlName
                                                     });

                return Json(new { data = filteredSubControls }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private async Task PopulateDropdowns()
        {
            ViewBag.AccountHeadID = new SelectList(await _accountSettingFacade.AccountHeadRepository.GetAllAsync(), "AccountHeadID", "AccountHeadName");
            ViewBag.AccountActivityID = new SelectList(await _accountSettingFacade.AccountActivityRepository.GetAllAsync(), "AccountActivityID", "Name");
        }
    }
}