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
        private readonly HttpClientHelper _httpClientHelper;

        private const string DEFAULT_IMAGE_PATH = "~/Content/StuffLogo/customer.png";

        public SalePaymentController(SessionHelper sessionHelper, HttpClientHelper httpClientHelper)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(httpClientHelper));
        }

        // GET: PurchasePayment
        public async Task<ActionResult> RemainingPaymentList()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClientHelper.GetAsync<dynamic>(
                    $"salepayment/remainingPaymentList/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");

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
                var response = await _httpClientHelper.GetAsync<dynamic>($"salepayment/salePaymentHistory/{id.Value}");
                var returnDetails = await _httpClientHelper.GetAsync<dynamic>($"salepayment/getReturnSaleDetails/{id.Value}");

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
                var response = await _httpClientHelper.GetAsync<dynamic>($"salepayment/salePaymentHistory/{id.Value}");
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

                var response = await _httpClientHelper.PostAsync<dynamic>(
                    "salepayment/processPayment", paymentDto);

                if (response.Message == "RemainingAmountError")
                {
                    ViewBag.Message = response.Message;
                    var history = await _httpClientHelper.GetAsync<dynamic>($"salepayment/salePaymentHistory/{id.Value}");
                    ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(history);
                }

                TempData["Message"] = response.Message;
                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                var history = await _httpClientHelper.GetAsync<dynamic>($"salepayment/salePaymentHistory/{id.Value}");
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
                var response = await _httpClientHelper.GetAsync<dynamic>(
                    $"salepayment/customSalesHistory/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}/{fromDate.ToString("yyyy-MM-dd")}/{toDate.ToString("yyyy-MM-dd")}");

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
