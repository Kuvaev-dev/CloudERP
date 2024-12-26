using CloudERP.Helpers;
using Domain.EntryAccess;
using Domain.RepositoryAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly IPurchaseRepository _purchase;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierPaymentRepository _supplierPaymentRepository;
        private readonly ISupplierReturnInvoiceRepository _supplierReturnInvoiceRepository;
        private readonly ISupplierInvoiceDetailRepository _supplierInvoiceDetailRepository;
        private readonly IPurchaseEntry _paymentEntry;

        public PurchasePaymentController(SessionHelper sessionHelper, IPurchaseRepository purchase, ISupplierReturnInvoiceRepository supplierReturnInvoiceRepository, ISupplierPaymentRepository supplierPaymentRepository, IPurchaseEntry paymentEntry, ISupplierInvoiceRepository supplierInvoiceRepository, ISupplierRepository supplierRepository, ISupplierInvoiceDetailRepository supplierInvoiceDetailRepository)
        {
            _sessionHelper = sessionHelper;
            _purchase = purchase;
            _supplierReturnInvoiceRepository = supplierReturnInvoiceRepository;
            _supplierPaymentRepository = supplierPaymentRepository;
            _paymentEntry = paymentEntry;
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _supplierRepository = supplierRepository;
            _supplierInvoiceDetailRepository = supplierInvoiceDetailRepository;
        }

        // GET: PurchasePayment
        public async Task<ActionResult> RemainingPaymentList()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _purchase.RemainingPaymentList(_sessionHelper.CompanyID, _sessionHelper.BranchID);

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
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _purchase.PurchasePaymentHistory((int)id);
                var returnDetails = await _supplierReturnInvoiceRepository.GetReturnDetails((int)id);
                if (returnDetails != null)
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalInvoiceAmount = await _supplierInvoiceRepository.GetTotalAmountAsync((int)id);
                double totalPaidAmount = await _supplierPaymentRepository.GetTotalPaidAmount((int)id);
                remainingAmount = totalInvoiceAmount - totalPaidAmount;

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
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _purchase.PurchasePaymentHistory((int)id);
                var returnDetails = await _supplierReturnInvoiceRepository.GetReturnDetails((int)id);
                if (returnDetails != null && returnDetails.Any())
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalPaidAmount = 0;
                double totalInvoiceAmount = await _supplierInvoiceRepository.GetTotalAmountAsync((int)id);
                if (await _supplierPaymentRepository.GetByInvoiceIdAsync((int)id))
                {
                    totalPaidAmount = await _supplierPaymentRepository.GetTotalPaidAmount((int)id);
                }
                remainingAmount = totalInvoiceAmount - totalPaidAmount;

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
        public async Task<ActionResult> PaidAmount(int? id, float previousRemainingAmount, float paymentAmount)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = Resources.Messages.PurchasePaymentRemainingAmountError;
                    var list = await _purchase.PurchasePaymentHistory((int)id);
                    var returnDetails = await _supplierReturnInvoiceRepository.GetReturnDetails((int)id);
                    if (returnDetails != null && returnDetails.Any())
                    {
                        ViewData["ReturnPurchaseDetails"] = returnDetails;
                    }

                    double totalInvoiceAmount = await _supplierInvoiceRepository.GetTotalAmountAsync((int)id);
                    double totalPaidAmount = await _supplierPaymentRepository.GetTotalPaidAmount((int)id);
                    double remainingAmount = totalInvoiceAmount - totalPaidAmount;

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplierID = await _supplierInvoiceRepository.GetSupplierIdFromInvoice((int)id);
                var supplier = await _supplierRepository.GetByIdAsync(supplierID);
                var purchaseInvoice = await _supplierInvoiceRepository.GetByIdAsync((int)id);
                string message = await _paymentEntry.PurchasePayment(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, payinvoicenno, Convert.ToString(id), (float)purchaseInvoice.TotalAmount,
                    paymentAmount, Convert.ToString(supplier?.SupplierID), supplier?.SupplierName, previousRemainingAmount - paymentAmount);
                Session["Message"] = message;

                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> CustomPurchasesHistory(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = await _purchase.CustomPurchasesList(_sessionHelper.CompanyID, _sessionHelper.BranchID, FromDate, ToDate);

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
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                if (id != null)
                {
                    Session["BrchID"] = id;
                }

                var list = await _purchase.CustomPurchasesList(_sessionHelper.CompanyID, _sessionHelper.BrchID, FromDate, ToDate);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PurchaseItemDetail(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _supplierInvoiceDetailRepository.GetListByIdAsync((int)id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PrintPurchaseInvoice(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }
                
                return View(await _supplierInvoiceDetailRepository.GetListByIdAsync((int)id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}