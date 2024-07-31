using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web.Mvc;

[assembly: InternalsVisibleTo("CloudERP.Tests")]
namespace CloudERP.Controllers
{
    public class PurchasePaymentReturnController : Controller
    {
        private readonly CloudDBEntities _db;
        internal SP_Purchase _purchase;
        internal PurchaseEntry _purchaseEntry;

        public PurchasePaymentReturnController(CloudDBEntities db)
        {
            _db = db;
            _purchase = new SP_Purchase(_db);
            _purchaseEntry = new PurchaseEntry(_db);
        }

        // GET: PurchasePaymentReturn
        public ActionResult ReturnPurchasePendingAmount(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = _purchase.PurchaseReturnPaymentPending(id);
                
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult AllPurchasesPendingPayment()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                var list = _purchase.GetReturnPurchasesPaymentPending(companyID, branchID);
                
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult ReturnAmount(int? id)
        {
            try
            {
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
                    remainingAmount = _db.tblSupplierReturnInvoice.Find(id).TotalAmount;
                }

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;
                
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult ReturnAmount(int? id, float previousRemainingAmount, float paymentAmount)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = "Payment must be less than or equal to the previous remaining amount.";
                    var list = _db.tblSupplierReturnPayment.Where(r => r.SupplierReturnInvoiceID == id);
                    double remainingAmount = 0;
                    
                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }

                    if (remainingAmount == 0)
                    {
                        remainingAmount = _db.tblSupplierReturnInvoice.Find(id).TotalAmount;
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payinvoicenno = "RPP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = _db.tblSupplier.Find(_db.tblSupplierReturnInvoice.Find(id).SupplierID);
                var purchaseInvoice = _db.tblSupplierReturnInvoice.Find(id);
                var purchasePaymentDetails = _db.tblSupplierReturnPayment.Where(p => p.SupplierReturnInvoiceID == id);
                string message = _purchaseEntry.ReturnPurchasePayment(companyID, branchID, userID, payinvoicenno, purchaseInvoice.SupplierInvoiceID.ToString(), purchaseInvoice.SupplierReturnInvoiceID, (float)purchaseInvoice.TotalAmount,
                    paymentAmount, Convert.ToString(supplier.SupplierID), supplier.SupplierName, previousRemainingAmount - paymentAmount);
                Session["Message"] = message;
                
                return RedirectToAction("PurchasePaymentReturn", new { id });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}