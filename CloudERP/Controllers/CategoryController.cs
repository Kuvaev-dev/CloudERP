using System;
using System.Web.Mvc;
using CloudERP.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            var categories = _service.GetAll(companyID, branchID);
            return View(categories);
        }

        public ActionResult Details(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var userType = _service.GetById(id);
                return View(userType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult Create()
        {
            return View(new CategoryMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoryMV model)
        {
            if (ModelState.IsValid)
            {
                var category = new Domain.Models.Category
                {
                    CategoryName = model.CategoryName,
                    CompanyID = Convert.ToInt32(Session["CompanyID"]),
                    BranchID = Convert.ToInt32(Session["BranchID"]),
                    UserID = Convert.ToInt32(Session["UserID"])
                };

                _service.Create(category);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var category = _service.GetById(id);
            if (category == null) return HttpNotFound();

            var model = new CategoryMV
            {
                CategoryID = category.CategoryID,
                CategoryName = category.CategoryName
            };

            return View(model);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CategoryMV model)
        {
            if (ModelState.IsValid)
            {
                var accountHead = new Domain.Models.Category
                {
                    CategoryID = model.CategoryID,
                    CategoryName = model.CategoryName,
                    UserID = Convert.ToInt32(Session["UserID"])
                };

                _service.Update(accountHead);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}