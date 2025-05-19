using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Purchase.Payment
{
    public class PurchasePaymentReturnController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        public PurchasePaymentReturnController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
        }

        // GET: PurchasePaymentReturn/ReturnPurchasePendingAmount
        public async Task<ActionResult> ReturnPurchasePendingAmount()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<PurchaseInfo>>(
                    $"purchasepaymentreturnapi/getreturnpurchasependingamount" +
                    $"?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllPurchasesPendingPayment()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<PurchaseInfo>>(
                    $"purchasepaymentreturnapi/getreturnpurchasependingamount" +
                    $"?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> ReturnAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<SupplierReturnPayment>>(
                    $"purchasepaymentreturnapi/getsupplierreturnpayments?id={id}");

                double? remainingAmount = list?.Sum(item => item.RemainingBalance);
                if (remainingAmount == 0) return RedirectToAction("AllPurchasesPendingPayment");

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                var returnAmountDto = new PurchaseReturn
                {
                    InvoiceId = id,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaymentAmount = paymentAmount
                };

                bool isSuccess = await _httpClient.PostAsync("purchasepaymentreturnapi/processreturnamount", returnAmountDto);

                HttpContext.Session.SetString("Message", isSuccess ? "Payment processed successfully" : "Payment processing failed");

                return RedirectToAction("AllPurchasesPendingPayment");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}