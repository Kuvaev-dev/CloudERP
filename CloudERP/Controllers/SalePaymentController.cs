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

        public SalePaymentController(CloudDBEntities db, SP_Sale sale, SaleEntry saleEntry)
        {
            _db = db;
            _sale = sale;
            _saleEntry = saleEntry;
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return View("Error");
            }
        }

        public ActionResult PaidHistory(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || !id.HasValue)
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var list = _sale.SalePaymentHistory(id.Value);
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult PaidAmount(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || !id.HasValue)
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var list = _sale.SalePaymentHistory(id.Value);
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult PaidAmount(int? id, float previousRemainingAmount, float paidAmount)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])) || !id.HasValue)
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                if (paidAmount > previousRemainingAmount)
                {
                    ViewBag.Message = Resources.Messages.PurchasePaymentRemainingAmountError;
                    var list = _sale.SalePaymentHistory(id.Value);
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
                string message = _saleEntry.SalePayment(companyID, branchID, userID, payInvoiceNo, Convert.ToString(id), (float)purchaseInvoice.TotalAmount,
                    paidAmount, Convert.ToString(customer.CustomerID), customer.Customername, previousRemainingAmount - paidAmount);

                TempData["Message"] = message;

                return RedirectToAction("RemainingPaymentList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                var list = _sale.SalePaymentHistory(id.Value);
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                int branchID = (id.HasValue) ? id.Value : Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);
                var list = _sale.CustomSalesList(companyID, branchID, FromDate, ToDate);

                return View(list.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}