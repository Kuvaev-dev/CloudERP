using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Mapping;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class AccountSubControlController : Controller
    {
        private readonly IAccountSubControlService _service;
        private readonly IAccountControlService _controlService;
        private readonly IMapper<AccountSubControl, AccountSubControlMV> _mapper;

        public AccountSubControlController(IAccountSubControlService service, IAccountControlService controlService, IMapper<AccountSubControl, AccountSubControlMV> mapper)
        {
            _service = service;
            _controlService = controlService;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);

            var subControls = await _service.GetAllAsync(companyId, branchId);
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

            var accountHead = await _controlService.GetByIdAsync(model.AccountControlID);

            model.BranchID = branchId;
            model.CompanyID = companyId;
            model.UserID = userId;
            model.AccountControlList = null;
            model.AccountHeadID = accountHead.AccountHeadID;

            if (ModelState.IsValid)
            {
                await _service.CreateAsync(_mapper.MapToDomain(model));
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
            var subControl = await _service.GetByIdAsync(id.Value);
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
                await _service.UpdateAsync(_mapper.MapToDomain(model));
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