using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentReturnController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_Purchase purchase = new SP_Purchase();
        private readonly PurchaseEntry purchaseEntry = new PurchaseEntry();

        public PurchasePaymentReturnController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: PurchasePaymentReturn
        public ActionResult ReturnPurchasePendingAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = purchase.PurchaseReturnPaymenPending(id);

            return View(list);
        }

        public ActionResult AllPurchasesPendingPayment()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = purchase.GetReturnPurchasesPaymentPending(companyID, branchID);

            return View(list);
        }

        public ActionResult ReturnAmount(int? id)
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

        [HttpPost]
        public ActionResult ReturnAmount(int? id, float previousRemainingAmount, float paymentAmount)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || string.IsNullOrEmpty(Convert.ToString(id)))
                {
                    return RedirectToAction("Login", "Home");
                }
                int companyID = 0;
                int branchID = 0;
                int userID = 0;
                branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
                companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
                userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = "Payment Must be Less Then or Equal to Previous Remaining Amount!";
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
                string message = purchaseEntry.ReturnPurchasePayment(companyID, branchID, userID, payinvoicenno, purchaseInvoice.SupplierInvoiceID.ToString(), purchaseInvoice.SupplierReturnInvoiceID, (float)purchaseInvoice.TotalAmount,
                    paymentAmount, Convert.ToString(supplier.SupplierID), supplier.SupplierName, previousRemainingAmount - paymentAmount);
                Session["Message"] = message;
                return RedirectToAction("PurchasePaymentReturn", new { id = id });
            }
            catch
            {
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
        }
    }
}