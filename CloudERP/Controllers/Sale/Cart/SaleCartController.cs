using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Sale.Cart
{
    public class SaleCartController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClientHelper;

        public SaleCartController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClientHelper)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(HttpClientHelper));
        }

        // GET: SaleCart/NewSale
        public async Task<ActionResult> NewSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var findDetail = await _httpClientHelper.GetAsync<List<SaleCartDetail>>(
                    $"sale-cart/new-sale-details/{_sessionHelper.BranchID}/{_sessionHelper.CompanyID}/{_sessionHelper.UserID}");

                ViewBag.TotalAmount = findDetail?.Sum(item => item.SaleQuantity * item.SaleUnitPrice);
                ViewBag.Products = await _httpClientHelper.GetAsync<List<Domain.Models.Stock>>(
                    $"stock/products/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");

                return View(findDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<IActionResult> GetProductDetails(int id)
        {
            try
            {
                var product = await _httpClientHelper.GetAsync<Domain.Models.Stock>(
                    $"sale-cart/product-details/{id}");

                return Json(new { data = product?.SaleUnitPrice });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return Json(new { error = "Product details fetching error" });
            }
        }

        // POST: SaleCart/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem(int PID, int Qty, float Price)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var checkQty = await _httpClientHelper.GetAsync<Domain.Models.Stock>(
                    $"stock/products/{PID}");

                if (Qty > checkQty?.Quantity)
                {
                    ViewBag.Message = "Insufficient quantity available.";
                    return RedirectToAction("NewSale");
                }

                var newItem = new SaleCartDetail()
                {
                    ProductID = PID,
                    SaleQuantity = Qty,
                    SaleUnitPrice = Price,
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    UserID = _sessionHelper.UserID
                };

                var responseMessage = await _httpClientHelper.PostAsync<SaleCartDetail>(
                    "sale-cart/add-item", newItem);

                ViewBag.Message = responseMessage;

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/DeleteConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var responseMessage = await _httpClientHelper.PutAsync<string>($"sale-cart/delete-item/{id}", new { });

                ViewBag.Message = responseMessage;
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/CancelSale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var responseMessage = await _httpClientHelper.PostAsync<string>(
                    $"sale-cart/cancel-sale/{_sessionHelper.BranchID}/{_sessionHelper.CompanyID}/{_sessionHelper.UserID}", new { });

                ViewBag.Message = responseMessage;

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/SaleConfirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaleConfirm(SaleConfirm saleConfirmDto)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClientHelper.PostAsync<int>(
                    "sale-cart/confirm-sale", saleConfirmDto);

                if (response)
                {
                    return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = response });
                }

                TempData["ErrorMessage"] = response;
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}