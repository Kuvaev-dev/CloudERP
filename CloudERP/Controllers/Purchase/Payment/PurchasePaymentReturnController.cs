using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Purchase.Payment
{
    public class PurchasePaymentReturnController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClientHelper;

        public PurchasePaymentReturnController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClientHelper)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(HttpClientHelper));
        }

        // GET: PurchasePaymentReturn/ReturnPurchasePendingAmount
        public async Task<ActionResult> ReturnPurchasePendingAmount()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClientHelper.GetAsync<List<PurchasePaymentModel>>(
                    $"purchase-payment-return/return-purchase-pending-amount/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllPurchasesPendingPayment()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClientHelper.GetAsync<List<PurchasePaymentModel>>(
                    $"purchase-payment-return/return-purchase-pending-amount/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> ReturnAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClientHelper.GetAsync<List<SupplierReturnPayment>>(
                    $"purchase-payment-return/supplier-return-payments/{id}");

                double? remainingAmount = list?.Sum(item => item.RemainingBalance);
                if (remainingAmount == 0) return RedirectToAction("AllPurchasesPendingPayment");

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReturnAmount(int id, float previousRemainingAmount, float paymentAmount)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var returnAmountDto = new PurchaseReturnAmount
                {
                    InvoiceId = id,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaymentAmount = paymentAmount
                };

                bool isSuccess = await _httpClientHelper.PostAsync<PurchaseReturnAmount>("purchase-payment-return/process-return-payment", returnAmountDto);

                HttpContext.Session.SetString("Message", isSuccess ? "Payment processed successfully" : "Payment processing failed");

                return RedirectToAction("AllPurchasesPendingPayment");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}