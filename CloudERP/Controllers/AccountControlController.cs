using System;
using System.Web.Mvc;
using Domain.Services;
using Domain.ViewModels;

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
                _service.Create(model);
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
                _service.Update(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}