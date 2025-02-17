using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models.FinancialModels;

namespace CloudERP.Controllers
{
    public class SalePaymentController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        private const string DEFAULT_IMAGE_PATH = "~/Content/StuffLogo/customer.png";

        public SalePaymentController(
            SessionHelper sessionHelper, 
            HttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
        }

        // GET: PurchasePayment
        public async Task<ActionResult> RemainingPaymentList()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync<dynamic>(
                    $"sale-payment/remaining-payment-list/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");

                return View(response.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return View("Error");
            }
        }

        public async Task<ActionResult> PaidHistory(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync<dynamic>($"sale-payment/sale-payment-history/{id.Value}");
                var returnDetails = await _httpClient.GetAsync<dynamic>($"sale-payment/get-return-sale-details/{id.Value}");

                if (returnDetails?.Count() > 0)
                {
                    ViewData["ReturnSaleDetails"] = returnDetails;
                }

                ViewBag.PreviousRemainingAmount = response.PreviousRemainingAmount;
                ViewBag.InvoiceID = id;

                return View(response.History);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PaidAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync<dynamic>($"sale-payment/sale-payment-history/{id.Value}");
                var remainingAmount = response.RemainingAmount;

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(response.History);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PaidAmount(int? id, float previousRemainingAmount, float paidAmount)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var paymentDto = new SalePayment
                {
                    InvoiceId = id.Value,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaidAmount = paidAmount
                };

                var response = await _httpClient.PostAsync<dynamic>(
                    $"sale-payment/process-payment/{_sessionHelper.BranchID}/{_sessionHelper.CompanyID}/{_sessionHelper.UserID}", paymentDto);

                if (!response)
                {
                    ViewBag.Message = "Unexpected error";
                    var history = await _httpClient.GetAsync<dynamic>($"sale-payment/sale-payment-history/{id.Value}");
                    ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(history);
                }

                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                var history = await _httpClient.GetAsync<dynamic>($"sale-payment/sale-payment-history/{id.Value}");
                ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                ViewBag.InvoiceID = id;
                return View(history);
            }
        }

        public async Task<ActionResult> CustomSalesHistory(DateTime fromDate, DateTime toDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync<dynamic>(
                    $"sale-payment/custom-sales-history/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}");

                return View(response.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}
