using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.Models.Purchase;
using Localization.CloudERP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        private const string DEFAULT_IMAGE_PATH = "~/Content/StuffLogo/supplier.png";

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
                var list = await _httpClient.GetAsync<List<PurchasePayment>>(
                    $"purchase-payment/remaining-payment-list/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
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
                var list = await _httpClient.GetAsync<List<PurchasePaymentModel>>(
                    $"purchase-payment/paid-history/{id}");
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
                var list = await _httpClient.GetAsync<List<PurchasePaymentModel>>(
                    $"purchase-payment/paid-history/{id}");

                double remainingAmount = list.LastOrDefault()?.RemainingBalance ?? 0;

                if (remainingAmount == 0)
                {
                    remainingAmount = await _httpClient.GetAsync<double>(
                        $"purchase-payment/total-amount/{id}");
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
        public async Task<ActionResult> PaidAmount(int? id, float previousRemainingAmount, float paymentAmount)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var paymentDto = new PurchasePayment
                {
                    InvoiceId = id.Value,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaidAmount = paymentAmount
                };

                var success = await _httpClient.PostAsync(
                    "purchase-payment/process-payment", paymentDto);

                if (!success)
                {
                    var list = await _httpClient.GetAsync<List<PurchasePaymentModel>>(
                        $"purchase-payment/paid-history/{id}");
                    ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                var list = await _httpClient.GetAsync<List<PurchasePaymentModel>>(
                    $"purchase-payment/paid-history/{id}");
                ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }

        public async Task<ActionResult> CustomPurchasesHistory(DateTime fromDate, DateTime toDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _httpClient.GetAsync<List<Purchase>>(
                    $"purchase-payment/custom-purchases-history/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}/{fromDate:yyyy-MM-dd}/{toDate:yyyy-MM-dd}");

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
                    $"purchase-payment/purchase-item-detail/{id}");
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
                    $"purchase-payment/purchase-invoice/{id}");

                if (invoiceDetails?.Any() != true)
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
                    InvoiceItems = invoiceDetails.ToList(),
                    ReturnInvoices = invoiceDetails
                        .Where(i => i.SupplierReturnInvoiceDetail.Any())
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