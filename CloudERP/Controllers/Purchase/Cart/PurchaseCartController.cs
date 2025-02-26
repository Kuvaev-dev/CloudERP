using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Resources;

namespace CloudERP.Controllers.Purchase.Cart
{
    public class PurchaseCartController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public PurchaseCartController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
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
                var details = await _httpClient.GetAsync<PurchaseCartDetail[]>(
                    $"purchasecartapi/getpurchasecartdetails?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}&userId={_sessionHelper.UserID}");

                ViewBag.TotalAmount = details?.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);
                return View(details);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
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
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
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
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
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
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
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

                var result = await _httpClient.PostAndReturnAsync<object>("purchasecartapi/confirmpurchase", purchaseConfirmDto);

                if (result is not null)
                {
                    dynamic response = result;
                    if (response.id is int purchaseId)
                    {
                        return RedirectToAction("PrintPurchaseInvoice", "PurchasePayment", new { id = purchaseId });
                    }
                }

                TempData["ErrorMessage"] = "Purchase confirm error";
                return RedirectToAction("NewPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("NewPurchase");
            }
        }
    }
}