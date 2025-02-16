using CloudERP.Helpers;
using Domain.Models.FinancialModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SalePaymentReturnController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClientHelper;

        public SalePaymentReturnController(SessionHelper sessionHelper, HttpClientHelper httpClientHelper)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(httpClientHelper));
        }

        // GET: SalePaymentReturn
        public async Task<ActionResult> ReturnSalePendingAmount()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClientHelper.GetAsync<dynamic>(
                    $"sale-payment-return/return-sale-pending-amount?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(response);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllReturnSalesPendingAmount()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClientHelper.GetAsync<dynamic>(
                    $"sale-payment-return/return-sale-pending-amount?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(response);
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
                var response = await _httpClientHelper.GetAsync<dynamic>(
                    $"sale-payment-return/return-amount?invoiceID={id.Value}");

                double remainingAmount = response.RemainingAmount;
                if (remainingAmount == 0)
                    return RedirectToAction("AllReturnSalesPendingAmount");

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(response.Payments);
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
                var paymentReturnDto = new SalePaymentReturn
                {
                    InvoiceId = id,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaymentAmount = paymentAmount
                };

                var response = await _httpClientHelper.PostAsync<dynamic>(
                    $"sale-payment-return/process-return-amount?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&userId={_sessionHelper.UserID}", paymentReturnDto);

                Session["SaleMessage"] = response.Message;

                return RedirectToAction("PurchasePaymentReturn", new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}