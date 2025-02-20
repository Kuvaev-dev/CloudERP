using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Stock
{
    public class StockController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public StockController(
            HttpClientHelper httpClient,
            SessionHelper sessionHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Stock
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var stocks = await _httpClient.GetAsync<List<Domain.Models.Stock>>($"stock/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                return View(stocks);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
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
                if (id == null) return RedirectToAction("EP404", "EP");

                var stock = await _httpClient.GetAsync<Domain.Models.Stock>($"stock/{id}/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                if (stock == null) return RedirectToAction("EP404", "EP");

                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
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
                var categories = await _httpClient.GetAsync<List<Category>>($"category/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");

                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

                return View(new Domain.Models.Stock());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Domain.Models.Stock model)
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
                    var success = await _httpClient.PostAsync<Domain.Models.Stock>("stock/create", model);
                    if (success) return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
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
                if (id == null) return RedirectToAction("EP404", "EP");

                var stock = await _httpClient.GetAsync<Domain.Models.Stock>($"stock/{id}/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                if (stock == null) return RedirectToAction("EP404", "EP");

                var categories = await _httpClient.GetAsync<List<Category>>($"category/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");

                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Stock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Domain.Models.Stock model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PutAsync<Domain.Models.Stock>($"stock/update/{model.ProductID}", model);
                    if (success) return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/ProductQuality
        public async Task<ActionResult> ProductQuality()
        {
            try
            {
                var products = await _httpClient.GetAsync<List<Domain.Models.Stock>>($"stock/productquality/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}