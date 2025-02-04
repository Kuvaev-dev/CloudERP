using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services.Purchase;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IPurchaseService _purchaseService;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly IPurchasePaymentService _purchasePaymentService;

        private const string DEFAULT_IMAGE_PATH = "~/Content/EmployeePhoto/Default/default.png";

        public PurchasePaymentController(
            SessionHelper sessionHelper, 
            IPurchaseRepository purchaseRepository,
            IPurchaseService purchaseService,
            ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository, 
            IPurchasePaymentService purchasePaymentService,
            ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(IPurchaseRepository));
            _purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(IPurchaseService));
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(ISupplierInvoiceDetailRepository));
            _purchasePaymentService = purchasePaymentService ?? throw new ArgumentNullException(nameof(IPurchasePaymentService));
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository ?? throw new ArgumentNullException(nameof(ISupplierReturnInvoiceRepository));
        }

        // GET: PurchasePayment
        public async Task<ActionResult> RemainingPaymentList()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _purchaseRepository.RemainingPaymentList(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PaidHistory(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _purchasePaymentService.GetPurchasePaymentHistoryAsync(id.Value);
                var returnDetails = await _supplierReturnInvoiceRepository.GetReturnDetails(id.Value);

                if (returnDetails != null && returnDetails.Count() > 0)
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double totalInvoiceAmount = await _purchasePaymentService.GetTotalAmountByIdAsync(id.Value);
                double totalPaidAmount = await _purchasePaymentService.GetTotalPaidAmountByIdAsync(id.Value);
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
                var list = await _purchasePaymentService.GetPurchasePaymentHistoryAsync(id.Value);
                double remainingAmount = list.LastOrDefault()?.RemainingBalance ?? 0;

                if (remainingAmount == 0)
                {
                    remainingAmount = await _purchasePaymentService.GetTotalAmountByIdAsync(id.Value);
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
        public async Task<ActionResult> PaidAmount(int? id, float previousRemainingAmount, float paymentAmount)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                PurchasePayment paymentDto = new PurchasePayment
                {
                    InvoiceId = id.Value,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaidAmount = paymentAmount
                };

                string message = await _purchasePaymentService.ProcessPaymentAsync(
                    _sessionHelper.CompanyID,
                    _sessionHelper.BranchID,
                    _sessionHelper.UserID,
                    paymentDto);

                if (message == Resources.Messages.PurchasePaymentRemainingAmountError)
                {
                    ViewBag.Message = message;
                    var list = await _purchasePaymentService.GetPurchasePaymentHistoryAsync(id.Value);
                    ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                Session["Message"] = message;
                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                var list = await _purchasePaymentService.GetPurchasePaymentHistoryAsync(id.Value);
                ViewBag.PreviousRemainingAmount = previousRemainingAmount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }

        public async Task<ActionResult> CustomPurchasesHistory(DateTime FromDate, DateTime ToDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _purchaseRepository.CustomPurchasesList(_sessionHelper.CompanyID, _sessionHelper.BranchID, FromDate, ToDate);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SubCustomPurchasesHistory(DateTime FromDate, DateTime ToDate, int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _purchaseRepository.CustomPurchasesList(_sessionHelper.CompanyID, id ?? _sessionHelper.BranchID, FromDate, ToDate);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PurchaseItemDetail(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var purchaseDetail = await _purchaseService.GetPurchaseItemDetailAsync(id);
                return View(purchaseDetail);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PrintPurchaseInvoice(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var invoiceDetails = await _supplierInvoiceDetailRepository.GetListByIdAsync(id);

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
                    InvoiceNo = firstItem.CustomerInvoiceNo,
                    InvoiceDate = firstItem.CustomerInvoiceDate.ToString("dd/MM/yyyy"),
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}