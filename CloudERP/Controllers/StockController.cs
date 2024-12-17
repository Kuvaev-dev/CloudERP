using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class StockController : Controller
    {
        private readonly IStockService _stockService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper<Stock, StockMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public StockController(IStockService stockService, ICategoryService categoryService, IMapper<Stock, StockMV> mapper, SessionHelper sessionHelper)
        {
            _stockService = stockService;
            _categoryService = categoryService;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        // GET: Stock
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var stocks = await _stockService.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(stocks);
        }

        // GET: Stock/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var stock = await _stockService.GetByIdAsync(id.Value);
            if (stock == null)
                return HttpNotFound();

            return View(stock);
        }

        // GET: Stock/Create
        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var categories = await _categoryService.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

            return View(new StockMV());
        }

        // POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StockMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;
            model.BranchID = _sessionHelper.BranchID;
            model.UserID = _sessionHelper.UserID;

            if (ModelState.IsValid)
            {
                var existingStock = await _stockService.GetByProductNameAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, model.ProductName);
                if (existingStock == null)
                {
                    var stock = _mapper.MapToDomain(model);
                    await _stockService.CreateAsync(stock);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = Resources.Messages.AlreadyExists;
                }
            }

            var categories = await _categoryService.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName", model.CategoryID);

            return View(model);
        }

        // GET: Stock/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var stock = await _stockService.GetByIdAsync(id.Value);
            if (stock == null)
                return HttpNotFound();

            var categories = await _categoryService.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName", stock.CategoryID);

            return View(stock);
        }

        // POST: Stock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StockMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            int userID = _sessionHelper.UserID;
            model.UserID = userID;

            if (ModelState.IsValid)
            {
                var existingStock = await _stockService.GetByProductNameAsync(model.CompanyID, model.BranchID, model.ProductName);
                if (existingStock == null || existingStock.ProductID == model.ProductID)
                {
                    var stock = _mapper.MapToDomain(model);
                    await _stockService.UpdateAsync(stock);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = Resources.Messages.AlreadyExists;
                }
            }

            var categories = await _categoryService.GetAllAsync(model.CompanyID, model.BranchID);
            ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName", model.CategoryID);

            return View(model);
        }
    }
}