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
    public class AccountControlController : Controller
    {
        private readonly IAccountControlService _service;
        private readonly IAccountHeadService _headService;
        private readonly IMapper<AccountControl, AccountControlMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public AccountControlController(IAccountControlService service, IAccountHeadService headService, IMapper<AccountControl, AccountControlMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _headService = headService;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            var accountControls = await _service.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(accountControls);
        }

        public async Task<ActionResult> Create()
        {
            var model = new AccountControlMV
            {
                AccountHeadList = await GetAccountHeadList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountControlMV model)
        {
            if (ModelState.IsValid)
            {
                model.BranchID = _sessionHelper.BranchID;
                model.CompanyID = _sessionHelper.CompanyID;
                model.UserID = _sessionHelper.UserID;
                model.AccountHeadList = await GetAccountHeadList();

                await _service.CreateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var accountHeads = await _headService.GetAllAsync();
            var accountControl = await _service.GetByIdAsync(id.Value);
            if (accountControl == null) return HttpNotFound();

            var model = _mapper.MapToViewModel(accountControl);
            model.BranchID = _sessionHelper.BranchID;
            model.CompanyID = _sessionHelper.CompanyID;
            model.UserID = _sessionHelper.UserID;
            model.AccountHeadList = accountHeads
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName,
                    Selected = ah.AccountHeadID == accountControl.AccountHeadID
                }).ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountControlMV model)
        {
            if (ModelState.IsValid)
            {   
                await _service.UpdateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }

            model.AccountHeadList = await GetAccountHeadList();

            return View(model);
        }

        public async Task<IEnumerable<SelectListItem>> GetAccountHeadList()
        {
            var accountHeads = await _headService.GetAllAsync();
            return accountHeads
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName
                })
                .ToList();
        }
    }
}