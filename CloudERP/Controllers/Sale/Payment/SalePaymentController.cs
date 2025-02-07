using System;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using CloudERP.Helpers;
using Domain.Models.FinancialModels;
using CloudERP.Models;
using Domain.Facades;

namespace CloudERP.Controllers
{
    public class SalePaymentController : Controller
    {
        private readonly SalePaymentFacade _salePaymentFacade;
        private readonly SessionHelper _sessionHelper;

        private const string DEFAULT_IMAGE_PATH = "~/Content/EmployeePhoto/Default/default.png";

        public SalePaymentController(
            SalePaymentFacade salePaymentFacade,
            SessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _salePaymentFacade = salePaymentFacade ?? throw new ArgumentNullException(nameof(SalePaymentFacade));
        }

        // GET: PurchasePayment
        public async Task<ActionResult> RemainingPaymentList()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _salePaymentFacade.SaleRepository.RemainingPaymentList(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return View("Error");
            }
        }

        public async Task<ActionResult> PaidHistory(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _salePaymentFacade.SalePaymentService.GetSalePaymentHistoryAsync(id.Value);
                var returnDetails = await _salePaymentFacade.CustomerReturnInvoiceRepository.GetListByIdAsync(id.Value);

                if (returnDetails != null && returnDetails.Count() > 0)
                {
                    ViewData["ReturnSaleDetails"] = returnDetails;
                }

                double totalInvoiceAmount = await _salePaymentFacade.SalePaymentService.GetTotalAmountByIdAsync(id.Value);
                double totalPaidAmount = await _salePaymentFacade.SalePaymentService.GetTotalPaidAmountByIdAsync(id.Value);
                double remainingAmount = totalInvoiceAmount - totalPaidAmount;

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PaidAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _salePaymentFacade.SalePaymentService.GetSalePaymentHistoryAsync(id.Value);
                double remainingAmount = list.LastOrDefault()?.RemainingBalance ?? 0;

                if (remainingAmount == 0)
                {
                    remainingAmount = await _salePaymentFacade.SalePaymentService.GetTotalAmountByIdAsync(id.Value);
                }

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                SalePayment paymentDto = new SalePayment
                {
                    InvoiceId = id.Value,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaidAmount = paidAmount
                };

                string message = await _salePaymentFacade.SalePaymentService.ProcessPaymentAsync(
                    paymentDto,
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    _sessionHelper.UserID);

                if (message == Resources.Messages.PurchasePaymentRemainingAmountError)
                {
                    ViewBag.Message = message;
                    var list = await _salePaymentFacade.SalePaymentService.GetSalePaymentHistoryAsync(id.Value);
                    ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                TempData["Message"] = message;
                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                var list = await _salePaymentFacade.SalePaymentService.GetSalePaymentHistoryAsync(id.Value);
                ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }

        public async Task<ActionResult> CustomSalesHistory(DateTime FromDate, DateTime ToDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _salePaymentFacade.SaleRepository.CustomSalesList(_sessionHelper.CompanyID, _sessionHelper.BranchID, FromDate, ToDate);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SubCustomSalesHistory(DateTime FromDate, DateTime ToDate, int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _salePaymentFacade.SaleRepository.CustomSalesList(_sessionHelper.CompanyID, id ?? _sessionHelper.BranchID, FromDate, ToDate);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SaleItemDetail(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var saleDetail = await _salePaymentFacade.SaleService.GetSaleItemDetailAsync(id);
                return View(saleDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PrintSaleInvoice(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var invoiceDetails = await _salePaymentFacade.CustomerInvoiceDetailRepository.GetListByIdAsync(id);

                if (invoiceDetails?.Any() != true)
                    return RedirectToAction("EP500", "EP");

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
                    InvoiceNo = firstItem.CustomerInvoiceNo,
                    InvoiceDate = firstItem.CustomerInvoiceDate.ToString("dd/MM/yyyy"),
                    TotalCost = invoiceDetails.Sum(i => i.ItemCost),
                    InvoiceItems = invoiceDetails.ToList(),
                    ReturnInvoices = invoiceDetails
                        .Where(i => i.CustomerReturnInvoiceDetail.Any())
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}