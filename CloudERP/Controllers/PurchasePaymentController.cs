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
    public class PurchasePaymentController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();
        private SP_Purchase purchase = new SP_Purchase();
        private PurchaseEntry paymentEntry = new PurchaseEntry();

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
            var returnDetails = db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
            if (returnDetails != null)
            {
                if (returnDetails.Count > 0)
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }
            }
            double remainingAmount = 0;
            double totalInvoiceAmount = db.tblSupplierInvoice.Find(id).TotalAmount;
            double totalPaidAmount = db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
            remainingAmount = totalInvoiceAmount - totalPaidAmount;
            //foreach (var item in list)
            //{
            //    remainingAmount = item.RemainingBalance;
            //}
            //if (remainingAmount == 0)
            //{
            //    remainingAmount = db.tblSupplierInvoice.Find(id).TotalAmount;
            //}
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
            var list = purchase.PurchasePaymentHistory((int)id);
            var returnDetails = db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
            if (returnDetails != null)
            {
                if (returnDetails.Count > 0)
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }
            }
            double remainingAmount = 0;
            double totalPaidAmount = 0;
            double totalInvoiceAmount = db.tblSupplierInvoice.Find(id).TotalAmount;
            if (db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).FirstOrDefault() != null)
            {
                totalPaidAmount = db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
            }
            remainingAmount = totalInvoiceAmount - totalPaidAmount;
            //foreach (var item in list)
            //{
            //    remainingAmount = item.RemainingBalance;
            //}
            //if (remainingAmount == 0)
            //{
            //    remainingAmount = db.tblSupplierInvoice.Find(id).TotalAmount;
            //}
            ViewBag.PreviousRemainingAmount = remainingAmount;
            ViewBag.InvoiceID = id;
            return View(list.ToList());
        }

        [HttpPost]
        public ActionResult PaidAmount(int? id, float previousRemainingAmount, float paymentAmount)
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
                    var list = purchase.PurchasePaymentHistory((int)id);
                    var returnDetails = db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
                    if (returnDetails != null)
                    {
                        if (returnDetails.Count > 0)
                        {
                            ViewData["ReturnPurchaseDetails"] = returnDetails;
                        }
                    }
                    double remainingAmount = 0;
                    double totalInvoiceAmount = db.tblSupplierInvoice.Find(id).TotalAmount;
                    double totalPaidAmount = db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
                    remainingAmount = totalInvoiceAmount - totalPaidAmount;
                    //foreach (var item in list)
                    //{
                    //    remainingAmount = item.RemainingBalance;
                    //}
                    //if (remainingAmount == 0)
                    //{
                    //    remainingAmount = db.tblSupplierInvoice.Find(id).TotalAmount;
                    //}
                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }
                string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = db.tblSupplier.Find(db.tblSupplierInvoice.Find(id).SupplierID);
                var purchaseInvoice = db.tblSupplierInvoice.Find(id);
                var purchasePaymentDetails = db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id);
                string message = paymentEntry.PurchasePayment(companyID, branchID, userID, payinvoicenno, Convert.ToString(id), (float)purchaseInvoice.TotalAmount,
                    paymentAmount, Convert.ToString(supplier.SupplierID), supplier.SupplierName, previousRemainingAmount - paymentAmount);
                Session["Message"] = message;
                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Please Try Again!";
                var list = purchase.PurchasePaymentHistory((int)id);
                double remainingAmount = 0;
                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }
                if (remainingAmount == 0)
                {
                    remainingAmount = db.tblSupplierInvoice.Find(id).TotalAmount;
                }
                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;
                return View(list);
            }
        }

        public ActionResult CustomPurchasesHistory(DateTime FromDate, DateTime ToDate)
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
            var list = purchase.CustomPurchasesList(companyID, branchID, FromDate, ToDate);
            return View(list.ToList());
        }

        public ActionResult SubCustomPurchasesHistory(DateTime FromDate, DateTime ToDate, int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            if (id != null)
            {
                Session["SubBranchID"] = id;
            }
            branchID = Convert.ToInt32(Convert.ToString(Session["SubBranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = purchase.CustomPurchasesList(companyID, branchID, FromDate, ToDate);
            return View(list.ToList());
        }

        public ActionResult PurchaseItemDetail(int? id)
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
            var list = db.tblSupplierInvoiceDetail.Where(i => i.SupplierInvoiceID == id);
            return View(list.ToList());
        }

        public ActionResult PrintPurchaseInvoice(int? id)
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
            var list = db.tblSupplierInvoiceDetail.Where(i => i.SupplierInvoiceID == id);
            return View(list.ToList());
        }
    }
}