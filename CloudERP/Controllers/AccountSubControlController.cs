using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class AccountSubControlController : Controller
    {
        private readonly IAccountSubControlService _service;
        private readonly IAccountControlService _controlService;

        public AccountSubControlController(IAccountSubControlService service, IAccountControlService controlService)
        {
            _service = service;
            _controlService = controlService;
        }

        public ActionResult Index()
        {
            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);

            var subControls = _service.GetAll(companyId, branchId);
            return View(subControls);
        }

        public async Task<ActionResult> Create()
        {
            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);

            var model = new AccountSubControlMV
            {
                AccountControlList = await GetAccountControlList(companyId, branchId)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSubControlMV model)
        {
            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);
            int userId = Convert.ToInt32(Session["UserID"]);

            model.BranchID = branchId;
            model.CompanyID = companyId;
            model.UserID = userId;
            model.AccountControlList = await GetAccountControlList(companyId, branchId);

            if (ModelState.IsValid)
            {
                var subControl = new AccountSubControl
                {
                    AccountSubControlName = model.AccountSubControlName,
                    AccountControlID = model.AccountControlID,
                    CompanyID = model.CompanyID,
                    BranchID = model.BranchID,
                    UserID = model.UserID,
                    AccountHeadID = _controlService.GetById(model.AccountControlID).AccountHeadID
                };

                _service.Create(subControl);
                return RedirectToAction("Index");
            }

            model.AccountControlList = await GetAccountControlList(companyId, branchId);

            return View(model);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);
            int userId = Convert.ToInt32(Session["UserID"]);

            if (id == null) return RedirectToAction("Index");

            var accountControls = await _controlService.GetAllAsync(companyId, branchId);
            var subControl = _service.GetById(id.Value);
            if (subControl == null) return HttpNotFound();

            var model = new AccountSubControlMV
            {
                AccountSubControlID = subControl.AccountSubControlID,
                AccountSubControlName = subControl.AccountSubControlName,
                AccountControlID = subControl.AccountControlID,
                CompanyID = subControl.CompanyID,
                BranchID = subControl.BranchID,
                UserID = subControl.UserID,
                AccountControlList = accountControls
                    .Select(ac => new SelectListItem
                    {
                        Value = ac.AccountControlID.ToString(),
                        Text = ac.AccountControlName,
                        Selected = ac.AccountControlID == subControl.AccountControlID
                    }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSubControlMV model)
        {
            if (ModelState.IsValid)
            {
                var subControl = new AccountSubControl
                {
                    AccountSubControlID = model.AccountSubControlID,
                    AccountSubControlName = model.AccountSubControlName,
                    AccountControlID = model.AccountControlID,
                    CompanyID = model.CompanyID,
                    BranchID = model.BranchID,
                    UserID = model.UserID,
                    AccountHeadID = _controlService.GetById(model.AccountControlID).AccountHeadID
                };

                _service.Update(subControl);
                return RedirectToAction("Index");
            }

            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);

            model.AccountControlList = await GetAccountControlList(companyId, branchId);

            return View(model);
        }

        public async Task<IEnumerable<SelectListItem>> GetAccountControlList(int companyId, int branchId)
        {
            var accountControls = await _controlService.GetAllAsync(companyId, branchId);
            return accountControls
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName
                })
                .ToList();
        }
    }
}