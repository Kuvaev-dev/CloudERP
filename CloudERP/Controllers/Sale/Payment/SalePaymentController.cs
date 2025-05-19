using CloudERP.Models;
using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Sale.Payment
{
    public class SalePaymentController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        private const string DEFAULT_IMAGE_PATH = "~/StuffLogo/customer.png";

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
                var response = await _httpClient.GetAsync<IEnumerable<SaleInfo>>(
                    $"salepaymentapi/getremainingpaymentlist" +
                    $"?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(response);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return View("Error");
            }
        }

        public async Task<ActionResult> PaidHistory(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<SaleInfo>>($"salepaymentapi/getpaidhistory?id={id}");

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

        public async Task<ActionResult> PaidAmount(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<SaleInfo>>(
                    $"salepaymentapi/getpaidhistory?id={id}");

                double remainingAmount = list?.LastOrDefault()?.RemainingBalance ?? 0;

                if (remainingAmount == 0)
                {
                    remainingAmount = await _httpClient.GetAsync<double>(
                        $"salepaymentapi/gettotalamount?id={id}");
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
        public async Task<ActionResult> PaidAmount(int id, float previousRemainingAmount, float paidAmount)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var paymentDto = new SaleAmount
                {
                    InvoiceId = id,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaidAmount = paidAmount,
                    CompanyID = _sessionHelper.CompanyID,
                    BranchID = _sessionHelper.BranchID,
                    UserID = _sessionHelper.UserID
                };

                var success = await _httpClient.PostAsync(
                    "salepaymentapi/processpayment", paymentDto);

                if (!success)
                {
                    var list = await _httpClient.GetAsync<IEnumerable<SaleInfo>>(
                        $"salepaymentapi/getpaidhistory?id={id}");
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

        public async Task<ActionResult> CustomSalesHistory(DateTime fromDate, DateTime toDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<SaleInfo>>(
                    $"salepaymentapi/getcustomsaleshistory" +
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

        public async Task<ActionResult> SubCustomSalesHistory(DateTime fromDate, DateTime toDate, int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<IEnumerable<SaleInfo>>(
                    $"salepaymentapi/getcustomsaleshistory" +
                    $"?companyId={_sessionHelper.CompanyID}&branchId={id}" +
                    $"&fromDate={fromDate:yyyy-MM-dd}&toDate={toDate:yyyy-MM-dd}");

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SaleItemDetail(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var saleDetail = await _httpClient.GetAsync<SaleItemDetailDto>(
                    $"salepaymentapi/getsaleitemdetail?id={id}");
                return View(saleDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PrintSaleInvoice(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var invoiceDetails = await _httpClient.GetAsync<IEnumerable<CustomerInvoiceDetail>>(
                    $"salepaymentapi/getsaleinvoice?id={id}");

                if (invoiceDetails == null) return RedirectToAction("EP500", "EP");

                var firstItem = invoiceDetails.First();
                var customer = firstItem.Customer;
                var branch = firstItem.Branch;

                var viewModel = new SaleInvoiceMV
                {
                    CustomerName = customer.Customername,
                    CustomerContact = customer.CustomerContact,
                    CustomerArea = customer.CustomerArea,
                    CustomerLogo = DEFAULT_IMAGE_PATH,
                    CompanyName = firstItem.CompanyName,
                    CompanyLogo = firstItem.CompanyLogo,
                    BranchName = branch.BranchName,
                    BranchContact = branch.BranchContact,
                    BranchAddress = branch.BranchAddress,
                    InvoiceNo = firstItem.CustomerInvoice.InvoiceNo,
                    InvoiceDate = firstItem.CustomerInvoiceDate.ToString("dd/MM/yyyy"),
                    TotalCost = invoiceDetails.Sum(i => i.ItemCost),
                    InvoiceItems = [.. invoiceDetails],
                    ReturnInvoices = invoiceDetails
                        .Where(i => i.CustomerReturnInvoiceDetail.Count != 0)
                        .Select(i => new ReturnSaleInvoiceMV
                        {
                            ReturnInvoiceNo = i.CustomerReturnInvoiceDetail.First().InvoiceNo,
                            ReturnInvoiceDate = i.CustomerReturnInvoiceDetail.First().InvoiceDate.ToString("dd/MM/yyyy"),
                            ReturnItems = i.CustomerReturnInvoiceDetail
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
