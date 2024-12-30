using CloudERP.Facades;
using CloudERP.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly PurchasePaymentFacade _purchasePaymentFacade;

        public PurchasePaymentController(SessionHelper sessionHelper, PurchasePaymentFacade purchasePaymentFacade)
        {
            _sessionHelper = sessionHelper;
            _purchasePaymentFacade = purchasePaymentFacade;
        }

        // GET: PurchasePayment
        public async Task<ActionResult> RemainingPaymentList()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _purchasePaymentFacade.PurchaseRepository.RemainingPaymentList(_sessionHelper.CompanyID, _sessionHelper.BranchID);

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
                var list = await _purchasePaymentFacade.PurchaseRepository.PurchasePaymentHistory((int)id);
                var returnDetails = await _purchasePaymentFacade.SupplierReturnInvoiceRepository.GetReturnDetails((int)id);
                if (returnDetails != null)
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalInvoiceAmount = await _purchasePaymentFacade.SupplierInvoiceRepository.GetTotalAmountAsync((int)id);
                double totalPaidAmount = await _purchasePaymentFacade.SupplierPaymentRepository.GetTotalPaidAmount((int)id);
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
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _purchasePaymentFacade.PurchaseRepository.PurchasePaymentHistory((int)id);
                var returnDetails = await _purchasePaymentFacade.SupplierReturnInvoiceRepository.GetReturnDetails((int)id);
                if (returnDetails != null && returnDetails.Any())
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalPaidAmount = 0;
                double totalInvoiceAmount = await _purchasePaymentFacade.SupplierInvoiceRepository.GetTotalAmountAsync((int)id);
                if (await _purchasePaymentFacade.SupplierPaymentRepository.GetByInvoiceIdAsync((int)id))
                {
                    totalPaidAmount = await _purchasePaymentFacade.SupplierPaymentRepository.GetTotalPaidAmount((int)id);
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
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = Resources.Messages.PurchasePaymentRemainingAmountError;
                    var list = await _purchasePaymentFacade.PurchaseRepository.PurchasePaymentHistory((int)id);
                    var returnDetails = await _purchasePaymentFacade.SupplierReturnInvoiceRepository.GetReturnDetails((int)id);
                    if (returnDetails != null && returnDetails.Any())
                    {
                        ViewData["ReturnPurchaseDetails"] = returnDetails;
                    }

                    double totalInvoiceAmount = await _purchasePaymentFacade.SupplierInvoiceRepository.GetTotalAmountAsync((int)id);
                    double totalPaidAmount = await _purchasePaymentFacade.SupplierPaymentRepository.GetTotalPaidAmount((int)id);
                    double remainingAmount = totalInvoiceAmount - totalPaidAmount;

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplierID = await _purchasePaymentFacade.SupplierInvoiceRepository.GetSupplierIdFromInvoice((int)id);
                var supplier = await _purchasePaymentFacade.SupplierRepository.GetByIdAsync(supplierID);
                var purchaseInvoice = await _purchasePaymentFacade.SupplierInvoiceRepository.GetByIdAsync((int)id);
                string message = await _purchasePaymentFacade.PurchaseEntry.PurchasePayment(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, payinvoicenno, Convert.ToString(id), (float)purchaseInvoice.TotalAmount,
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
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _purchasePaymentFacade.PurchaseRepository.CustomPurchasesList(_sessionHelper.CompanyID, _sessionHelper.BranchID, FromDate, ToDate);

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
                if (id != null)
                {
                    Session["BrchID"] = id;
                }

                var list = await _purchasePaymentFacade.PurchaseRepository.CustomPurchasesList(_sessionHelper.CompanyID, _sessionHelper.BrchID, FromDate, ToDate);

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
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _purchasePaymentFacade.SupplierInvoiceDetailRepository.GetListByIdAsync((int)id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> PrintPurchaseInvoice(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {    
                return View(await _purchasePaymentFacade.SupplierInvoiceDetailRepository.GetListByIdAsync((int)id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}