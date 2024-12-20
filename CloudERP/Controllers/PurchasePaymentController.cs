using CloudERP.Helpers;
using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SessionHelper _sessionHelper;
        private readonly IPurchaseRepository _purchase;
        private readonly IPurchaseEntry _paymentEntry;

        public PurchasePaymentController(CloudDBEntities db, SessionHelper sessionHelper, IPurchaseRepository purchase, IPurchaseEntry paymentEntry)
        {
            _db = db;
            _sessionHelper = sessionHelper;
            _purchase = purchase;
            _paymentEntry = paymentEntry;
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
                var returnDetails = _db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
                if (returnDetails != null && returnDetails.Any())
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalInvoiceAmount = _db.tblSupplierInvoice.Find(id)?.TotalAmount ?? 0;
                double totalPaidAmount = _db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
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
                var returnDetails = _db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
                if (returnDetails != null && returnDetails.Any())
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalPaidAmount = 0;
                double totalInvoiceAmount = _db.tblSupplierInvoice.Find(id)?.TotalAmount ?? 0;
                if (_db.tblSupplierPayment.Any(p => p.SupplierInvoiceID == id))
                {
                    totalPaidAmount = _db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
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
                    var returnDetails = _db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
                    if (returnDetails != null && returnDetails.Any())
                    {
                        ViewData["ReturnPurchaseDetails"] = returnDetails;
                    }

                    double totalInvoiceAmount = _db.tblSupplierInvoice.Find(id)?.TotalAmount ?? 0;
                    double totalPaidAmount = _db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
                    double remainingAmount = totalInvoiceAmount - totalPaidAmount;

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = _db.tblSupplier.Find(_db.tblSupplierInvoice.Find(id)?.SupplierID);
                var purchaseInvoice = _db.tblSupplierInvoice.Find(id);
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

        public ActionResult PurchaseItemDetail(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = _db.tblSupplierInvoiceDetail.Where(i => i.SupplierInvoiceID == id);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult PrintPurchaseInvoice(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = _db.tblSupplierInvoiceDetail.Where(i => i.SupplierInvoiceID == id);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}