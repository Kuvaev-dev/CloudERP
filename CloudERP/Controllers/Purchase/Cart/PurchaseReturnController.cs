using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Purchase.Cart
{
    public class PurchaseReturnController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public PurchaseReturnController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public ActionResult FindPurchase()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(new SupplierInvoice());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> FindPurchase(string invoiceID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                HttpContext.Session.SetString("InvoiceNo", string.Empty);
                HttpContext.Session.SetString("ReturnMessage", string.Empty);

                var response = await _httpClient.GetAsync<dynamic>($"invoice/{invoiceID}");
                if (response == null)
                {
                    TempData["ErrorMessage"] = "Invoice not found.";
                    return RedirectToAction("FindPurchase");
                }

                ViewBag.InvoiceDetails = response.invoiceDetails;
                return View(response.invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReturnConfirm(PurchaseReturnConfirmVM model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                HttpContext.Session.SetString("SaleInvoiceNo", string.Empty);
                HttpContext.Session.SetString("SaleReturnMessage", string.Empty);

                var returnConfirmDto = new PurchaseReturnConfirm
                {
                    SupplierInvoiceID = model.SupplierInvoiceID,
                    IsPayment = model.IsPayment,
                    ProductIDs = model.ProductReturns.Select(x => x.ProductID).ToList(),
                    ReturnQty = model.ProductReturns.Select(x => x.ReturnQty).ToList(),
                    BranchID = _sessionHelper.BranchID,
                    CompanyID = _sessionHelper.CompanyID,
                    UserID = _sessionHelper.UserID
                };

                var response = await _httpClient.PostAsync<PurchaseReturnConfirm>("return", returnConfirmDto);
                if (response)
                {
                    return RedirectToAction("AllPurchasesPendingPayment", "PurchasePaymentReturn");
                }

                return RedirectToAction("FindPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}