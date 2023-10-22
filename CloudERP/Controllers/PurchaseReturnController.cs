using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseReturnController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: PurchaseReturn
        public ActionResult FindPurchase()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var purchaseInvoice = db.tblSupplierInvoice.Find(0);
            return View(purchaseInvoice);
        }

        [HttpPost]
        public ActionResult FindPurchase(string invoiceID)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var purchaseInvoice = db.tblSupplierInvoice.Where(p => p.InvoiceNo == invoiceID).FirstOrDefault();
            return View(purchaseInvoice);
        }
    }
}