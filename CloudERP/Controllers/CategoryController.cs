using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Mapping;
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

        public CategoryController(ICategoryService service, IMapper<Category, CategoryMV> mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            var categories = await _service.GetAllAsync(companyID, branchID);
            return View(categories);
        }

        public async Task<ActionResult> Details(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
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
                model.CompanyID = Convert.ToInt32(Session["CompanyID"]);
                model.BranchID = Convert.ToInt32(Session["BranchID"]);
                model.UserID = Convert.ToInt32(Session["UserID"]);

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
                model.CompanyID = Convert.ToInt32(Session["CompanyID"]);
                model.BranchID = Convert.ToInt32(Session["BranchID"]);
                model.UserID = Convert.ToInt32(Session["UserID"]);

                await _service.UpdateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}