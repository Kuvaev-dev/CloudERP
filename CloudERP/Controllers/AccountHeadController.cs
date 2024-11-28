using System;
using System.Web.Mvc;
using CloudERP.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class AccountHeadController : Controller
    {
        private readonly IAccountHeadService _service;

        public AccountHeadController(IAccountHeadService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            var accountHeads = _service.GetAll();
            return View(accountHeads);
        }

        public ActionResult Create()
        {
            return View(new AccountHeadMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AccountHeadMV model)
        {
            if (ModelState.IsValid)
            {
                var accountHead = new Domain.Models.AccountHead
                {
                    AccountHeadName = model.AccountHeadName,
                    Code = model.Code,
                    UserID = Convert.ToInt32(Session["UserID"])
                };

                _service.Create(accountHead);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var accountHead = _service.GetById(id);
            if (accountHead == null) return HttpNotFound();

            var model = new AccountHeadMV
            {
                AccountHeadID = accountHead.AccountHeadID,
                AccountHeadName = accountHead.AccountHeadName,
                Code = accountHead.Code,
                UserID = accountHead.UserID
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AccountHeadMV model)
        {
            if (ModelState.IsValid)
            {
                var accountHead = new Domain.Models.AccountHead
                {
                    AccountHeadID = model.AccountHeadID,
                    AccountHeadName = model.AccountHeadName,
                    Code = model.Code,
                    UserID = model.UserID
                };

                _service.Update(accountHead);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}