using System.Linq;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using Domain.RepositoryAccess;
using Domain.Models;

namespace CloudERP.Controllers
{
    public class AccountSettingController : Controller
    {
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IAccountControlRepository _accountControlRepository;
        private readonly IAccountSubControlRepository _accountSubControlRepository;
        private readonly IAccountHeadRepository _accountHeadRepository;
        private readonly IAccountActivityRepository _accountActivityRepository;
        private readonly SessionHelper _sessionHelper;

        public AccountSettingController(
            IAccountSettingRepository accountSettingRepository,
            IAccountControlRepository accountControlRepository,
            IAccountSubControlRepository accountSubControlRepository,
            IAccountHeadRepository accountHeadRepository,
            IAccountActivityRepository accountActivityRepository,
            SessionHelper sessionHelper)
        {
            _accountSettingRepository = accountSettingRepository;
            _accountControlRepository = accountControlRepository;
            _accountSubControlRepository = accountSubControlRepository;
            _accountHeadRepository = accountHeadRepository;
            _accountActivityRepository = accountActivityRepository;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var settings = await _accountSettingRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(settings);
        }

        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new AccountSetting());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSetting model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;

                await _accountSettingRepository.AddAsync(model);

                TempData["SuccessMessage"] = "Account Setting successfully created.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var setting = await _accountSettingRepository.GetByIdAsync(id);
            if (setting == null)
                return HttpNotFound();

            return View(setting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSetting model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;

                await _accountSettingRepository.UpdateAsync(model);

                TempData["SuccessMessage"] = "Account Setting successfully updated.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> GetAccountControls(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var controls = await _accountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
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
                var subControls = await _accountSubControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
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
            ViewBag.AccountHeadID = new SelectList(await _accountHeadRepository.GetAllAsync(), "AccountHeadID", "AccountHeadName");
            ViewBag.AccountActivityID = new SelectList(await _accountActivityRepository.GetAllAsync(), "AccountActivityID", "Name");
        }
    }
}