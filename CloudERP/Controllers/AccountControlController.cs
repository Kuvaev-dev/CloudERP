using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class AccountControlController : Controller
    {
        private readonly IAccountControlService _service;
        private readonly IAccountHeadService _headService;

        public AccountControlController(IAccountControlService service, IAccountHeadService headService)
        {
            _service = service;
            _headService = headService;
        }

        public async Task<ActionResult> Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);

            var accountControls = await _service.GetAllAsync(companyId, branchId);
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
            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);
            int userId = Convert.ToInt32(Session["UserID"]);

            model.BranchID = branchId;
            model.CompanyID = companyId;
            model.UserID = userId;
            model.AccountHeadList = await GetAccountHeadList();

            if (ModelState.IsValid)
            {
                var accountControl = new Domain.Models.AccountControl
                {
                    AccountControlName = model.AccountControlName,
                    CompanyID = model.CompanyID,
                    BranchID = model.BranchID,
                    UserID = model.UserID,
                    AccountHeadID = model.AccountHeadID
                };

                _service.Create(accountControl);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var accountHeads = await _headService.GetAllAsync();
            var accountControl = _service.GetById(id.Value);
            if (accountControl == null) return HttpNotFound();

            var model = new AccountControlMV
            {
                AccountControlID = accountControl.AccountControlID,
                AccountControlName = accountControl.AccountControlName,
                AccountHeadID = accountControl.AccountHeadID,
                BranchID = Convert.ToInt32(Session["BranchID"]),
                CompanyID = Convert.ToInt32(Session["CompanyID"]),
                UserID = Convert.ToInt32(Session["UserID"]),
                AccountHeadList = accountHeads
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName,
                    Selected = ah.AccountHeadID == accountControl.AccountHeadID
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountControlMV model)
        {
            if (ModelState.IsValid)
            {
                var accountControl = new Domain.Models.AccountControl
                {
                    AccountControlID = model.AccountControlID,
                    AccountControlName = model.AccountControlName,
                    AccountHeadID = model.AccountHeadID,
                    BranchID = model.BranchID,
                    CompanyID = model.CompanyID,
                    UserID = model.UserID
                };

                try
                {
                    _service.Update(accountControl);
                    return RedirectToAction("Index");
                }
                catch (KeyNotFoundException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
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