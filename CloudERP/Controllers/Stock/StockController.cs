using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.Stock
{
    public class StockController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public StockController(
            IHttpClientHelper httpClient,
            ISessionHelper sessionHelper)
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
                var stocks = await _httpClient.GetAsync<List<Domain.Models.Stock>>(
                    $"stockapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(stocks);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

                var stock = await _httpClient.GetAsync<Domain.Models.Stock>($"stockapi/getbyid?id={id}");
                if (stock == null) return RedirectToAction("EP404", "EP");

                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                var categories = await _httpClient.GetAsync<List<Category>>(
                    $"categoryapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

                return View(new Domain.Models.Stock() { ExpiryDate = DateTime.Now, Manufacture = DateTime.Now });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                var success = await _httpClient.PostAsync("stockapi/create", model);
                if (success) return RedirectToAction("Index");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                var categories = await _httpClient.GetAsync<List<Category>>(
                    $"categoryapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

                var stock = await _httpClient.GetAsync<Domain.Models.Stock>($"stockapi/getbyid?id={id}");
                if (stock == null) return RedirectToAction("EP404", "EP");

                var categories = await _httpClient.GetAsync<List<Category>>(
                    $"categoryapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

                return View(stock);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.UserID = _sessionHelper.UserID;

                var success = await _httpClient.PutAsync($"stockapi/update?id={model.ProductID}", model);
                if (success) return RedirectToAction("Index");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                var categories = await _httpClient.GetAsync<List<Category>>(
                    $"categoryapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                ViewBag.CategoryID = new SelectList(categories, "CategoryID", "CategoryName");

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/ProductQuality
        public async Task<ActionResult> ProductQuality()
        {
            try
            {
                var products = await _httpClient.GetAsync<IEnumerable<ProductQuality>>(
                    $"stockapi/getproductquality?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}