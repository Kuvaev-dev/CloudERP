using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;
using Utils.Helpers;

namespace CloudERP.Controllers.Sale.Cart
{
    public class SaleCartController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        public SaleCartController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
        }

        // GET: SaleCart/NewSale
        public async Task<ActionResult> NewSale()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var findDetail = await _httpClient.GetAsync<IEnumerable<SaleCartDetail>>(
                    $"salecartapi/getsalecartdetails" +
                    $"?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}&userId={_sessionHelper.UserID}");

                ViewBag.TotalAmount = findDetail?.Sum(item => item.SaleQuantity * item.SaleUnitPrice);
                ViewBag.Products = await _httpClient.GetAsync<List<Domain.Models.Stock>>(
                    $"stockapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

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
                var product = await _httpClient.GetAsync<Domain.Models.Stock>(
                    $"salecartapi/getproductdetails?id={id}");

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
                var checkQty = await _httpClient.GetAsync<Domain.Models.Stock>($"stockapi/getbyid?id={PID}");

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

                var success = await _httpClient.PostAsync("salecartapi/additem", newItem);
                if (success) ViewBag.Message = "Item added successfully";

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
                var success = await _httpClient.PutAsync($"salecartapi/deleteitem?id={id}", new { });
                if (success) ViewBag.Message = "Item deleted successfully";

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SelectCustomer()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _httpClient.GetAsync<IEnumerable<Customer>>(
                    $"customerapi/getbysetting?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}"));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("NewPurchase");
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
                var responseMessage = await _httpClient.PostAsync(
                    $"salecartapi/cancelsale" +
                    $"?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}&userId={_sessionHelper.UserID}",
                    new { });

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
                saleConfirmDto.CompanyID = _sessionHelper.CompanyID;
                saleConfirmDto.BranchID = _sessionHelper.BranchID;
                saleConfirmDto.UserID = _sessionHelper.UserID;

                var result = await _httpClient.PostAndReturnAsync<int>(
                    "salecartapi/confirmsale", saleConfirmDto);
                if (result > 0)
                {
                    return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = result });
                }

                TempData["ErrorMessage"] = "Purchase return error";
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