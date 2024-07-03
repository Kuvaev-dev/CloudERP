using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Code;
using DatabaseAccess;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SalePaymentController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_Sale _sale;
        private readonly SaleEntry _saleEntry;

        public SalePaymentController(CloudDBEntities db)
        {
            _db = db;
            _sale = new SP_Sale(_db);
            _saleEntry = new SaleEntry(_db);
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

                var list = _sale.RemainingPaymentList(companyID, branchID);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error retrieving remaining payment list: " + ex.Message;
                return View("Error");
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

                var list = _sale.SalePaymentHistory((int)id);
                var returnDetails = _db.tblCustomerReturnInvoice.Where(r => r.CustomerInvoiceID == id).ToList();
               
                if (returnDetails != null && returnDetails.Count > 0)
                {
                    ViewData["ReturnSaleDetails"] = returnDetails;
                }

                double remainingAmount = 0;
                double totalInvoiceAmount = _db.tblCustomerInvoice.Find(id).TotalAmount;
                double totalPaidAmount = _db.tblCustomerPayment.Where(p => p.CustomerInvoiceID == id).Sum(p => p.PaidAmount);
                
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

                var list = _sale.SalePaymentHistory((int)id);
                double remainingAmount = 0;

                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }

                if (remainingAmount == 0)
                {
                    remainingAmount = _db.tblCustomerInvoice.Find(id).TotalAmount;
                }

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
        public ActionResult PaidAmount(int? id, float previousRemainingAmount, float paidAmount)
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

                if (paidAmount > previousRemainingAmount)
                {
                    ViewBag.Message = "Payment must be less than or equal to previous remaining amount!";
                    var list = _sale.SalePaymentHistory((int)id);
                    double remainingAmount = 0;
                    
                    foreach (var item in list)
                    {
                        remainingAmount = item.RemainingBalance;
                    }

                    if (remainingAmount == 0)
                    {
                        remainingAmount = _db.tblCustomerInvoice.Find(id).TotalAmount;
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payInvoiceNo = "INP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customer = _db.tblCustomer.Find(_db.tblCustomerInvoice.Find(id).CustomerID);
                var purchaseInvoice = _db.tblCustomerInvoice.Find(id);
                var purchasePaymentDetails = _db.tblCustomerPayment.Where(p => p.CustomerInvoiceID == id);
                string message = _saleEntry.SalePayment(companyID, branchID, userID, payInvoiceNo, Convert.ToString(id), (float)purchaseInvoice.TotalAmount,
                    paidAmount, Convert.ToString(customer.CustomerID), customer.Customername, previousRemainingAmount - paidAmount);
                Session["Message"] = message;
                
                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error processing payment: " + ex.Message;
                var list = _sale.SalePaymentHistory((int)id);
                double remainingAmount = 0;

                foreach (var item in list)
                {
                    remainingAmount = item.RemainingBalance;
                }

                if (remainingAmount == 0)
                {
                    remainingAmount = _db.tblCustomerInvoice.Find(id).TotalAmount;
                }

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;
                
                return View(list);
            }
        }

        public ActionResult CustomSalesHistory(DateTime FromDate, DateTime ToDate)
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

                var list = _sale.CustomSalesList(companyID, branchID, FromDate, ToDate);
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult SubCustomSalesHistory(DateTime FromDate, DateTime ToDate, int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = (id != null) ? Convert.ToInt32(id) : Convert.ToInt32(Session["SubBranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);
                var list = _sale.CustomSalesList(companyID, branchID, FromDate, ToDate);
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult SaleItemDetail(int? id)
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

                var list = _db.tblCustomerInvoiceDetail.Where(i => i.CustomerInvoiceID == id);
                
                return View(list.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult PrintSaleInvoice(int? id)
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

                var list = _db.tblCustomerInvoiceDetail.Where(i => i.CustomerInvoiceID == id);
                
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