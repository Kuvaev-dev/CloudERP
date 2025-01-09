using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class StockController : Controller
    {
        private readonly IStockRepository _stockRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly SessionHelper _sessionHelper;

        public StockController(
            IStockRepository stockRepository, 
            ICategoryRepository categoryRepository, 
            SessionHelper sessionHelper)
        {
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(SessionHelper));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(SessionHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Stock
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var stocks = await _stockRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                return View(stocks);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var stock = await _stockRepository.GetByIdAsync(id.Value);
                if (stock == null)
                    return HttpNotFound();

                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/Create
        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var categories = await _categoryRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

                return View(new Stock());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Stock model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var existingStock = await _stockRepository.GetByProductNameAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, model.ProductName);
                    if (existingStock == null)
                    {
                        await _stockRepository.AddAsync(model);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = Resources.Messages.AlreadyExists;
                    }
                }

                var categories = await _categoryRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName", model.CategoryID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var stock = await _stockRepository.GetByIdAsync(id.Value);
                if (stock == null)
                    return HttpNotFound();

                var categories = await _categoryRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName", stock.CategoryID);

                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Stock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Stock model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                int userID = _sessionHelper.UserID;
                model.UserID = userID;

                if (ModelState.IsValid)
                {
                    var existingStock = await _stockRepository.GetByProductNameAsync(model.CompanyID, model.BranchID, model.ProductName);
                    if (existingStock == null || existingStock.ProductID == model.ProductID)
                    {
                        await _stockRepository.UpdateAsync(model);

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = Resources.Messages.AlreadyExists;
                    }
                }

                var categories = await _categoryRepository.GetAllAsync(model.CompanyID, model.BranchID);
                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName", model.CategoryID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}