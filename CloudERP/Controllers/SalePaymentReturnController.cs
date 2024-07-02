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
        private readonly SP_Sale sale = new SP_Sale();
        private readonly SaleEntry saleEntry = new SaleEntry();

        public SalePaymentReturnController(CloudDBEntities db)
        {
            _db = db;
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
                var list = sale.SaleReturnAmountPending(id);
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
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
                var list = sale.GetReturnSaleAmountPending(companyID, branchID);
                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
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
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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
                var salePaymentDetails = _db.tblCustomerReturnPayment.Where(p => p.CustomerReturnInvoiceID == id);

                string message = saleEntry.ReturnSalePayment(companyID, branchID, userID, payInvoiceNo, saleInvoice.CustomerInvoiceID.ToString(), saleInvoice.CustomerReturnInvoiceID, (float)saleInvoice.TotalAmount,
                    paymentAmount, customer.CustomerID.ToString(), customer.Customername, previousRemainingAmount - paymentAmount);

                Session["SaleMessage"] = message;
                return RedirectToAction("PurchasePaymentReturn", new { id = id });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}