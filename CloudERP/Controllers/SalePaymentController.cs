using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Code;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SalePaymentController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();
        private SP_Sale sale = new SP_Sale();
        private SaleEntry saleEntry = new SaleEntry();

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
            var list = sale.RemainingPaymentList(companyID, branchID);
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
            var list = sale.SalePaymentHistory((int)id);
            var returnDetails = db.tblCustomerReturnInvoice.Where(r => r.CustomerInvoiceID == id).ToList();
            if (returnDetails != null)
            {
                if (returnDetails.Count > 0)
                {
                    ViewData["ReturnSaleDetails"] = returnDetails;
                }
            }
            double remainingAmount = 0;
            double totalInvoiceAmount = db.tblCustomerInvoice.Find(id).TotalAmount;
            double totalPaidAmount = db.tblCustomerPayment.Where(p => p.CustomerInvoiceID == id).Sum(p => p.PaidAmount);
            remainingAmount = totalInvoiceAmount - totalPaidAmount;
            ViewBag.PreviousRemainingAmount = remainingAmount;
            ViewBag.InvoiceID = id;
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
            var list = sale.SalePaymentHistory((int)id);
            double remainingAmount = 0;
            foreach (var item in list)
            {
                remainingAmount = item.RemainingBalance;
            }
            if (remainingAmount == 0)
            {
                remainingAmount = db.tblCustomerInvoice.Find(id).TotalAmount;
            }
            ViewBag.PreviousRemainingAmount = remainingAmount;
            ViewBag.InvoiceID = id;
            return View(list.ToList());
        }

        [HttpPost]
        public ActionResult PaidAmount(int? id, float previousRemainingAmount, float paidAmount)
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
                if (paidAmount > previousRemainingAmount)
                {
                    ViewBag.Message = "Payment Must be Less Then or Equal to Previous Remaining Amount!";
                    var list = sale.SalePaymentHistory((int)id);
                    double remainingAmount = 0;
                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }
                    if (remainingAmount == 0)
                    {
                        remainingAmount = db.tblCustomerInvoice.Find(id).TotalAmount;
                    }
                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }
                string payinvoicenno = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customer = db.tblCustomer.Find(db.tblCustomerInvoice.Find(id).CustomerID);
                var purchaseInvoice = db.tblCustomerInvoice.Find(id);
                var purchasePaymentDetails = db.tblCustomerPayment.Where(p => p.CustomerInvoiceID == id);
                string message = saleEntry.SalePayment(companyID, branchID, userID, payinvoicenno, Convert.ToString(id), (float)purchaseInvoice.TotalAmount,
                    paidAmount, Convert.ToString(customer.CustomerID), customer.Customername, previousRemainingAmount - paidAmount);
                Session["Message"] = message;
                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Please Try Again!";
                var list = sale.SalePaymentHistory((int)id);
                double remainingAmount = 0;
                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }
                if (remainingAmount == 0)
                {
                    remainingAmount = db.tblCustomerInvoice.Find(id).TotalAmount;
                }
                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }

        public ActionResult CustomSalesHistory(DateTime FromDate, DateTime ToDate)
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
            var list = sale.CustomSalesList(companyID, branchID, FromDate, ToDate);
            return View(list.ToList());
        }

        public ActionResult SaleItemDetail(int? id)
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
            var list = db.tblCustomerInvoiceDetail.Where(i => i.CustomerInvoiceID == id);
            return View(list.ToList());
        }
    }
}