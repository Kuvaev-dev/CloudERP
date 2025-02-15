using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;

namespace CloudERP.Controllers
{
    public class StockController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly SessionHelper _sessionHelper;
        private const string ApiBaseUrl = "http://localhost:5001/api/stock";

        public StockController(HttpClient httpClient, SessionHelper sessionHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: Stock
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync($"stock?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error retrieving stock data.";
                    return RedirectToAction("EP500", "EP");
                }

                var stocks = await response.Content.ReadAsAsync<IEnumerable<Stock>>();
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

                var response = await _httpClient.GetAsync($"stock/{id}?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error retrieving stock details.";
                    return RedirectToAction("EP500", "EP");
                }

                var stock = await response.Content.ReadAsAsync<Stock>();
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
                // Допустим, вы хотите получить список категорий с API
                var response = await _httpClient.GetAsync($"stock/categories?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error retrieving categories.";
                    return RedirectToAction("EP500", "EP");
                }

                var categories = await response.Content.ReadAsAsync<IEnumerable<Category>>();
                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

                return View(new Stock());
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
                    var response = await _httpClient.PostAsJsonAsync(ApiBaseUrl, model);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error creating stock item.";
                        return View(model);
                    }
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

                var response = await _httpClient.GetAsync($"stock/{id}?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error retrieving stock details for editing.";
                    return RedirectToAction("EP500", "EP");
                }

                var stock = await response.Content.ReadAsAsync<Stock>();
                if (stock == null) return RedirectToAction("EP404", "EP");

                // Получаем категории для отображения
                var categoriesResponse = await _httpClient.GetAsync($"stock/categories?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (!categoriesResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error retrieving categories.";
                    return RedirectToAction("EP500", "EP");
                }

                var categories = await categoriesResponse.Content.ReadAsAsync<IEnumerable<Category>>();
                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName", stock.CategoryID);

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
        public async Task<ActionResult> Edit(Stock model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var response = await _httpClient.PutAsJsonAsync($"stock/{model.ProductID}", model);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error updating stock item.";
                        return View(model);
                    }
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
                var response = await _httpClient.GetAsync($"stock/productquality?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Error retrieving product quality data.";
                    return RedirectToAction("EP500", "EP");
                }

                var products = await response.Content.ReadAsAsync<IEnumerable<ProductQuality>>();
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