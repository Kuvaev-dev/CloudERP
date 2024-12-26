using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly SessionHelper _sessionHelper;

        public CategoryController(ICategoryRepository categoryRepository, SessionHelper sessionHelper)
        {
            _categoryRepository = categoryRepository;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var categories = await _categoryRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(categories);
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var userType = await _categoryRepository.GetByIdAsync(id);
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
            return View(new Category());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                await _categoryRepository.AddAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return HttpNotFound();

            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category model)
        {
            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                await _categoryRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}