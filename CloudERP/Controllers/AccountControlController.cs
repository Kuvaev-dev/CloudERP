using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CloudERP.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class AccountControlController : Controller
    {
        private readonly IAccountControlService _service;

        public AccountControlController(IAccountControlService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);

            var accountControls = _service.GetAll(companyId, branchId);
            return View(accountControls);
        }

        public ActionResult Create()
        {
            var model = new AccountControlMV
            {
                AccountHeadList = _service.GetAllAccountHeads()
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName
                }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountControlMV model)
        {
            int companyId = Convert.ToInt32(Session["CompanyID"]);
            int branchId = Convert.ToInt32(Session["BranchID"]);
            int userId = Convert.ToInt32(Session["UserID"]);

            if (ModelState.IsValid)
            {
                var accountControl = new Domain.Models.AccountControl
                {
                    AccountControlName = model.AccountControlName,
                    CompanyID = companyId,
                    BranchID = branchId,
                    UserID = userId,
                    AccountHeadID = model.AccountHeadID
                };

                _service.Create(accountControl);
                return RedirectToAction("Index");
            }

            model.AccountHeadList = _service.GetAllAccountHeads()
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName
                }).ToList();

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var accountControl = _service.GetById(id);
            if (accountControl == null) return HttpNotFound();

            var model = new AccountControlMV
            {
                AccountControlID = accountControl.AccountControlID,
                AccountControlName = accountControl.AccountControlName,
                AccountHeadID = accountControl.AccountHeadID,
                AccountHeadList = _service.GetAllAccountHeads()
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
        public ActionResult Edit(AccountControlMV model)
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

            model.AccountHeadList = _service.GetAllAccountHeads()
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName
                }).ToList();

            return View(model);
        }
    }
}