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
    public class SalePaymentReturnController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();
        private SP_Sale sale = new SP_Sale();
        private SaleEntry saleEntry = new SaleEntry();

        // GET: SalePaymentReturn
        public ActionResult ReturnSalePendingAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = sale.SaleReturnAmountPending(id);

            return View(list);
        }

        public ActionResult AllReturnSalesPendingAmount()
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
            var list = sale.GetReturnSaleAmountPending(companyID, branchID);

            return View(list);
        }

        public ActionResult ReturnAmount(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var list = db.tblCustomerReturnPayment.Where(r => r.CustomerReturnInvoiceID == id);
            double remainingAmount = 0;
            foreach (var item in list)
            {
                remainingAmount = item.RemainingBalance;
                if (remainingAmount == 0)
                {
                    return RedirectToAction("AllReturnSalesPendingAmount");
                }
            }
            if (remainingAmount == 0)
            {
                remainingAmount = db.tblCustomerReturnInvoice.Find(id).TotalAmount;
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
                    var list = db.tblCustomerReturnPayment.Where(r => r.CustomerReturnInvoiceID == id);
                    double remainingAmount = 0;
                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }
                    if (remainingAmount == 0)
                    {
                        remainingAmount = db.tblCustomerReturnInvoice.Find(id).TotalAmount;
                    }
                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }
                string payinvoicenno = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customer = db.tblCustomer.Find(db.tblCustomerReturnInvoice.Find(id).CustomerID);
                var saleInvoice = db.tblCustomerReturnInvoice.Find(id);
                var salePaymentDetails = db.tblCustomerReturnPayment.Where(p => p.CustomerReturnInvoiceID == id);
                string message = saleEntry.ReturnSalePayment(companyID, branchID, userID, payinvoicenno, saleInvoice.CustomerInvoiceID.ToString(), saleInvoice.CustomerReturnInvoiceID, (float)saleInvoice.TotalAmount,
                    paymentAmount, Convert.ToString(customer.CustomerID), customer.Customername, previousRemainingAmount - paymentAmount);
                Session["SaleMessage"] = message;
                return RedirectToAction("PurchasePaymentReturn", new { id = id });
            }
            catch
            {
                var list = db.tblCustomerReturnPayment.Where(r => r.CustomerReturnInvoiceID == id);
                double remainingAmount = 0;
                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }
                if (remainingAmount == 0)
                {
                    remainingAmount = db.tblCustomerReturnInvoice.Find(id).TotalAmount;
                }
                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }
    }
}