using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_Purchase _purchase;
        private readonly PurchaseEntry _paymentEntry;

        public PurchasePaymentController(CloudDBEntities db)
        {
            _db = db;
            _purchase = new SP_Purchase(_db);
            _paymentEntry = new PurchaseEntry(_db);
        }

        // GET: PurchasePayment
        public ActionResult RemainingPaymentList()
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

                var list = _purchase.RemainingPaymentList(companyID, branchID);
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult PaidHistory(int? id)
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

                var list = _purchase.PurchasePaymentHistory((int)id);
                var returnDetails = _db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
                if (returnDetails != null && returnDetails.Any())
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalInvoiceAmount = _db.tblSupplierInvoice.Find(id).TotalAmount;
                double totalPaidAmount = _db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
                remainingAmount = totalInvoiceAmount - totalPaidAmount;

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult PaidAmount(int? id)
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

                var list = _purchase.PurchasePaymentHistory((int)id);
                var returnDetails = _db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
                if (returnDetails != null && returnDetails.Any())
                {
                    ViewData["ReturnPurchaseDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalPaidAmount = 0;
                double totalInvoiceAmount = _db.tblSupplierInvoice.Find(id).TotalAmount;
                if (_db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).FirstOrDefault() != null)
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
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = "Payment must be less than or equal to the previous remaining amount.";
                    var list = _purchase.PurchasePaymentHistory((int)id);
                    var returnDetails = _db.tblSupplierReturnInvoice.Where(r => r.SupplierInvoiceID == id).ToList();
                    if (returnDetails != null && returnDetails.Any())
                    {
                        ViewData["ReturnPurchaseDetails"] = returnDetails;
                    }

                    double totalInvoiceAmount = _db.tblSupplierInvoice.Find(id).TotalAmount;
                    double totalPaidAmount = _db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id).Sum(p => p.PaymentAmount);
                    double remainingAmount = totalInvoiceAmount - totalPaidAmount;

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;
                    return View(list);
                }

                string payinvoicenno = "PAY" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var supplier = _db.tblSupplier.Find(_db.tblSupplierInvoice.Find(id).SupplierID);
                var purchaseInvoice = _db.tblSupplierInvoice.Find(id);
                var purchasePaymentDetails = _db.tblSupplierPayment.Where(p => p.SupplierInvoiceID == id);
                string message = _paymentEntry.PurchasePayment(companyID, branchID, userID, payinvoicenno, Convert.ToString(id), (float)purchaseInvoice.TotalAmount,
                    paymentAmount, Convert.ToString(supplier.SupplierID), supplier.SupplierName, previousRemainingAmount - paymentAmount);
                Session["Message"] = message;
                
                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult CustomPurchasesHistory(DateTime FromDate, DateTime ToDate)
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

                var list = _purchase.CustomPurchasesList(companyID, branchID, FromDate, ToDate);
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult SubCustomPurchasesHistory(DateTime FromDate, DateTime ToDate, int? id)
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

                if (id != null)
                {
                    Session["SubBranchID"] = id;
                }

                branchID = Convert.ToInt32(Session["SubBranchID"]);
                var list = _purchase.CustomPurchasesList(companyID, branchID, FromDate, ToDate);
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult PurchaseItemDetail(int? id)
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

                var list = _db.tblSupplierInvoiceDetail.Where(i => i.SupplierInvoiceID == id);
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
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

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                var list = _db.tblSupplierInvoiceDetail.Where(i => i.SupplierInvoiceID == id);
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}