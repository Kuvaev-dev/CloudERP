using System;
using System.Collections.Generic;
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
            return View(new AccountControlMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountControlMV model)
        {
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

        public ActionResult Edit(int id)
        {
            var model = _service.GetById(id);
            if (model == null) return HttpNotFound();
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
            return View(model);
        }
    }
}