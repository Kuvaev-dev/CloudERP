using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;
        private readonly IMapper<Category, CategoryMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public CategoryController(ICategoryService service, IMapper<Category, CategoryMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var categories = await _service.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
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

                var userType = await _service.GetByIdAsync(id);
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
        public async Task<ActionResult> Create(CategoryMV model)
        {
            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                await _service.CreateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(category));
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CategoryMV model)
        {
            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                await _service.UpdateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}