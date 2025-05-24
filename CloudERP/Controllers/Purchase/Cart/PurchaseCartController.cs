using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Purchase.Cart
{
    public class PurchaseCartController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public PurchaseCartController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<ActionResult> NewPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var details = await _httpClient.GetAsync<IEnumerable<PurchaseCartDetail>>(
                    $"purchasecartapi/getpurchasecartdetails?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}&userId={_sessionHelper.UserID}");

                ViewBag.TotalAmount = details?.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);
                ViewBag.Products = await _httpClient.GetAsync<IEnumerable<Domain.Models.Stock>>(
                    $"stockapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(details);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<IActionResult> GetProductDetails(int id)
        {
            try
            {
                var product = await _httpClient.GetAsync<Domain.Models.Stock>(
                    $"purchasecartapi/getproductdetails?id={id}");

                return Json(new { data = product?.CurrentPurchaseUnitPrice });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return Json(new { error = "Product details fetching error" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem(int PID, int Qty, float Price)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var newItem = new PurchaseCartDetail
                {
                    ProductID = PID,
                    PurchaseQuantity = Qty,
                    PurchaseUnitPrice = Price,
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    UserID = _sessionHelper.UserID
                };

                var success = await _httpClient.PostAsync("purchasecartapi/additem", newItem);
                if (success) ViewBag.Message = "Item added successfully";

                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var success = await _httpClient.DeleteAsync($"purchasecartapi/deleteitem?id={id}");
                if (success) ViewBag.Message = "Deleted successfully";

                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        public async Task<ActionResult> SelectSupplier()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _httpClient.GetAsync<IEnumerable<Supplier>>(
                    $"supplierapi/getbysetting?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}"));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var success = await _httpClient.PostAsync($"purchasecartapi/cancelpurchase", new
                {
                    branchId = _sessionHelper.BranchID,
                    companyId = _sessionHelper.CompanyID,
                    userId = _sessionHelper.UserID
                });

                if (success) ViewBag.Message = "Purchase canceled";

                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PurchaseConfirm(PurchaseConfirm purchaseConfirmDto)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                purchaseConfirmDto.CompanyID = _sessionHelper.CompanyID;
                purchaseConfirmDto.BranchID = _sessionHelper.BranchID;
                purchaseConfirmDto.UserID = _sessionHelper.UserID;

                var result = await _httpClient.PostAndReturnAsync<Dictionary<string, int>>(
                    "purchasecartapi/confirmpurchase", purchaseConfirmDto);

                if (result != null && result.TryGetValue("invoiceId", out int invoiceId) && invoiceId > 0)
                {
                    return RedirectToAction("PrintPurchaseInvoice", "PurchasePayment", new { id = invoiceId });
                }

                TempData["ErrorMessage"] = "Purchase confirm error";
                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }
    }
}