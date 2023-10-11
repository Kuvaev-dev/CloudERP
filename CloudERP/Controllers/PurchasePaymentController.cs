using DatabaseAccess;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();
        private SP_Purchase purchase = new SP_Purchase();

        // GET: PurchasePayment
        public ActionResult RemainingPaymentList()
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
            var list = purchase.RemainingPaymentList(companyID, branchID);
            return View(list.ToList());
        }

        public ActionResult PaidHistory(int? id)
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
            var list = purchase.PurchasePaymentHistory((int)id);
            return View(list.ToList());
        }

        public ActionResult PaidAmount(int? id)
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
            var list = purchase.PurchasePaymentHistory((int)id);
            return View(list.ToList());
        }

        [HttpPost]
        public ActionResult PaidAmount()
        {
            //if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || string.IsNullOrEmpty(Convert.ToString(id)))
            //{
                return RedirectToAction("Login", "Home");
            //}
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            //var supplier = db.tblSupplier.Find(db.tblSupplierInvoice.Find(id).SupplierID);
            //var purchaseInvoice = db.tblSupplierInvoice.Find(id);
            //var purchasePaymentDetails = db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id);
            return View();
        }
    }
}