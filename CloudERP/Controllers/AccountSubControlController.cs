using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
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
        private readonly SessionHelper _sessionHelper;

        public AccountSubControlController(IAccountSubControlService service, IAccountControlService controlService, IMapper<AccountSubControl, AccountSubControlMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _controlService = controlService;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var subControls = await _service.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(subControls);
        }

        public async Task<ActionResult> Create()
        {
            var model = new AccountSubControlMV
            {
                AccountControlList = await GetAccountControlList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSubControlMV model)
        {
            var accountHead = await _controlService.GetByIdAsync(model.AccountControlID);

            model.BranchID = _sessionHelper.BranchID;
            model.CompanyID = _sessionHelper.CompanyID;
            model.UserID = _sessionHelper.UserID;
            model.AccountControlList = null;
            model.AccountHeadID = accountHead.AccountHeadID;

            if (ModelState.IsValid)
            {
                await _service.CreateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }

            model.AccountControlList = await GetAccountControlList();

            return View(model);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var subControl = await _service.GetByIdAsync(id.Value);
            if (subControl == null) return HttpNotFound();

            var model = _mapper.MapToViewModel(subControl);
            model.BranchID = _sessionHelper.BranchID;
            model.CompanyID = _sessionHelper.CompanyID;
            model.UserID = _sessionHelper.UserID;

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

            model.AccountControlList = await GetAccountControlList();

            return View(model);
        }

        public async Task<IEnumerable<SelectListItem>> GetAccountControlList()
        {
            var accountControls = await _controlService.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
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