using System.Linq;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class AccountSettingController : Controller
    {
        private readonly IAccountSettingService _service;
        private readonly IAccountControlService _controlService;
        private readonly IAccountSubControlService _subControlService;
        private readonly IAccountHeadService _headService;
        private readonly IAccountActivityService _activityService;
        private readonly IMapper<AccountSetting, AccountSettingMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public AccountSettingController(
            IAccountSettingService service,
            IAccountControlService controlService,
            IAccountSubControlService subControlService,
            IAccountHeadService headService,
            IAccountActivityService activityService,
            IMapper<AccountSetting, AccountSettingMV> mapper,
            SessionHelper sessionHelper)
        {
            _service = service;
            _controlService = controlService;
            _subControlService = subControlService;
            _headService = headService;
            _activityService = activityService;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var settings = await _service.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(settings);
        }

        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new AccountSettingMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSettingMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;

                var domainModel = _mapper.MapToDomain(model);
                await _service.CreateAsync(domainModel);

                TempData["SuccessMessage"] = "Account Setting successfully created.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var setting = await _service.GetByIdAsync(id);
            if (setting == null)
                return HttpNotFound();

            return View(_mapper.MapToViewModel(setting));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSettingMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;

                var domainModel = _mapper.MapToDomain(model);
                await _service.UpdateAsync(domainModel);

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
                var controls = await _controlService.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
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
                var subControls = await _subControlService.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
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
            ViewBag.AccountHeadID = new SelectList(await _headService.GetAllAsync(), "AccountHeadID", "AccountHeadName");
            ViewBag.AccountActivityID = new SelectList(await _activityService.GetAllAsync(), "AccountActivityID", "Name");
        }
    }
}