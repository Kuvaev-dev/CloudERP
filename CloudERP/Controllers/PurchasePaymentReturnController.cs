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
    public class PurchasePaymentReturnController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SessionHelper _sessionHelper;
        private readonly IPurchaseRepository _purchase;
        private readonly IPurchaseEntry _purchaseEntry;

        public PurchasePaymentReturnController(CloudDBEntities db, IPurchaseRepository purchase, IPurchaseEntry purchaseEntry, SessionHelper sessionHelper)
        {
            _db = db;
            _purchase = purchase;
            _purchaseEntry = purchaseEntry;
            _sessionHelper = sessionHelper;
        }

        // GET: PurchasePaymentReturn
        public async Task<ActionResult> ReturnPurchasePendingAmount(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _purchase.PurchaseReturnPaymentPending(id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllPurchasesPendingPayment()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _purchase.GetReturnPurchasesPaymentPending(_sessionHelper.CompanyID, _sessionHelper.BranchID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult ReturnAmount(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("AllPurchasesPendingPayment");
                }

                var list = _db.tblSupplierReturnPayment.Where(r => r.SupplierReturnInvoiceID == id);
                double remainingAmount = 0;

                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                    if (remainingAmount == 0)
                    {
                        return RedirectToAction("AllPurchasesPendingPayment");
                    }
                }

                if (remainingAmount == 0)
                {
                    remainingAmount = _db.tblSupplierReturnInvoice.Find(id)?.TotalAmount ?? 0;
                }

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> ReturnAmount(int? id, float previousRemainingAmount, float paymentAmount)
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
                    var list = _db.tblSupplierReturnPayment.Where(r => r.SupplierReturnInvoiceID == id);
                    double remainingAmount = 0;

                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }

                    if (remainingAmount == 0)
                    {
                        remainingAmount = _db.tblSupplierReturnInvoice.Find(id)?.TotalAmount ?? 0;
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = _db.tblSupplier.Find(_db.tblSupplierReturnInvoice.Find(id)?.SupplierID);
                var purchaseInvoice = _db.tblSupplierReturnInvoice.Find(id);
                
                Session["Message"] = await _purchaseEntry.ReturnPurchasePayment(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID, payinvoicenno, purchaseInvoice.SupplierInvoiceID.ToString(), purchaseInvoice.SupplierReturnInvoiceID, (float)purchaseInvoice.TotalAmount,
                    paymentAmount, Convert.ToString(supplier?.SupplierID), supplier?.SupplierName, previousRemainingAmount - paymentAmount); ;

                return RedirectToAction("PurchasePaymentReturn", new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}