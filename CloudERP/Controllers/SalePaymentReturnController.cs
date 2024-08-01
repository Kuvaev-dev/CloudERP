using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SalePaymentReturnController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_Sale _sale;
        private readonly SaleEntry _saleEntry;

        public SalePaymentReturnController(CloudDBEntities db)
        {
            _db = db;
            _sale = new SP_Sale(_db);
            _saleEntry = new SaleEntry(_db);
        }

        // GET: SalePaymentReturn
        public ActionResult ReturnSalePendingAmount(int? id)
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

                var list = _sale.GetReturnSaleAmountPending(companyID, branchID);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving return sale pending amounts: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult AllReturnSalesPendingAmount()
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

                var list = _sale.GetReturnSaleAmountPending(companyID, branchID);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving all return sales pending amounts: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult ReturnAmount(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (!id.HasValue)
                {
                    return RedirectToAction("AllReturnSalesPendingAmount");
                }

                var list = _db.tblCustomerReturnPayment.Where(r => r.CustomerReturnInvoiceID == id);

                double remainingAmount = list.Sum(item => item.RemainingBalance);
                if (remainingAmount == 0)
                {
                    return RedirectToAction("AllReturnSalesPendingAmount");
                }

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving return amount details: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult ReturnAmount(int? id, float previousRemainingAmount, float paymentAmount)
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

                if (paymentAmount > previousRemainingAmount)
                {
                    ViewBag.Message = "Payment must be less than or equal to the previous remaining amount!";
                    var list = _db.tblCustomerReturnPayment.Where(r => r.CustomerReturnInvoiceID == id);
                    double remainingAmount = list.Sum(item => item.RemainingBalance);

                    if (remainingAmount == 0)
                    {
                        remainingAmount = _db.tblCustomerReturnInvoice.Find(id).TotalAmount;
                    }

                    ViewBag.PreviousRemainingAmount = remainingAmount;
                    ViewBag.InvoiceID = id;

                    return View(list);
                }

                string payInvoiceNo = "RIP" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var customer = _db.tblCustomer.Find(_db.tblCustomerReturnInvoice.Find(id).CustomerID);
                var saleInvoice = _db.tblCustomerReturnInvoice.Find(id);

                string message = _saleEntry.ReturnSalePayment(companyID, branchID, userID, payInvoiceNo, saleInvoice.CustomerInvoiceID.ToString(), saleInvoice.CustomerReturnInvoiceID, (float)saleInvoice.TotalAmount,
                    paymentAmount, customer.CustomerID.ToString(), customer.Customername, previousRemainingAmount - paymentAmount);

                Session["SaleMessage"] = message;

                return RedirectToAction("PurchasePaymentReturn", new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while processing return amount: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}