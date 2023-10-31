using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentReturnController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();
        private SP_Purchase purchase = new SP_Purchase();
        private PurchaseEntry purchaseEntry = new PurchaseEntry();

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
    }
}