using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Purchase.Payment
{
    public class PurchasePaymentController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        private const string DEFAULT_IMAGE_PATH = "~/wwwroot/StuffLogo/supplier.png";

        public PurchasePaymentController(
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
                var list = await _httpClient.GetAsync<IEnumerable<PurchaseInfo>>(
                    $"purchasepaymentapi/getremainingpaymentlist?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PaidHistory(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<PurchaseInfo>>($"purchasepaymentapi/getpaidhistory?id={id}");

                ViewBag.PreviousRemainingAmount = await _httpClient.GetAsync<double>($"purchasepaymentapi/getremainingamount?id={id}");
                ViewBag.InvoiceID = id;

                return View(list);
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
                var list = await _httpClient.GetAsync<IEnumerable<PurchaseInfo>>(
                    $"purchasepaymentapi/getpaidhistory?id={id}");

                double remainingAmount = list?.LastOrDefault()?.RemainingBalance ?? 0;

                if (remainingAmount == 0)
                {
                    remainingAmount = await _httpClient.GetAsync<double>(
                        $"purchasepaymentapi/gettotalamount?id={id}");
                }

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
        public async Task<ActionResult> PaidAmount(int id, float previousRemainingAmount, float paymentAmount)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var paymentDto = new PurchaseAmount
                {
                    InvoiceId = id,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaidAmount = paymentAmount,
                    CompanyID = _sessionHelper.CompanyID,
                    BranchID = _sessionHelper.BranchID,
                    UserID = _sessionHelper.UserID
                };

                var success = await _httpClient.PostAsync(
                    "purchasepaymentapi/processpayment", paymentDto);

                if (!success)
                {
                    var list = await _httpClient.GetAsync<IEnumerable<PurchaseInfo>>(
                        $"purchasepaymentapi/getpaidhistory?id={id}");
                    ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return View();
            }
        }

        public async Task<ActionResult> CustomPurchasesHistory(DateTime fromDate, DateTime toDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<PurchaseInfo>>(
                    $"purchasepaymentapi/getcustompurchaseshistory" +
                    $"?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}" +
                    $"&fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PurchaseItemDetail(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var purchaseDetail = await _httpClient.GetAsync<PurchaseItemDetailDto>(
                    $"purchasepaymentapi/getpurchaseitemdetail?id={id}");
                return View(purchaseDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PrintPurchaseInvoice(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var invoiceDetails = await _httpClient.GetAsync<List<SupplierInvoiceDetail>>(
                    $"purchasepaymentapi/getpurchaseinvoice?id={id}");

                if (invoiceDetails == null || invoiceDetails.Count == 0)
                    return RedirectToAction("EP500", "EP");

                var firstItem = invoiceDetails.First();
                var supplier = firstItem.Supplier;
                var branch = firstItem.Branch;

                var viewModel = new PurchaseInvoiceMV
                {
                    SupplierName = supplier.SupplierName,
                    SupplierConatctNo = supplier.SupplierConatctNo,
                    SupplierAddress = supplier.SupplierAddress,
                    SupplierLogo = DEFAULT_IMAGE_PATH,
                    CompanyName = firstItem.CompanyName,
                    CompanyLogo = firstItem.CompanyLogo,
                    BranchName = branch.BranchName,
                    BranchContact = branch.BranchContact,
                    BranchAddress = branch.BranchAddress,
                    InvoiceNo = firstItem.SupplierInvoiceNo,
                    InvoiceDate = firstItem.SupplierInvoiceDate.ToString("dd/MM/yyyy"),
                    TotalCost = invoiceDetails.Sum(i => i.ItemCost),
                    InvoiceItems = [.. invoiceDetails],
                    ReturnInvoices = invoiceDetails
                        .Where(i => i.SupplierReturnInvoiceDetail.Count != 0)
                        .Select(i => new ReturnPurchaseInvoiceMV
                        {
                            ReturnInvoiceNo = i.SupplierReturnInvoiceDetail.First().InvoiceNo,
                            ReturnInvoiceDate = i.SupplierReturnInvoiceDetail.First().InvoiceDate.ToString("dd/MM/yyyy"),
                            ReturnItems = i.SupplierReturnInvoiceDetail
                        }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}