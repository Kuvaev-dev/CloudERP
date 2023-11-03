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
    public class SaleReturnController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();
        private SP_Sale sale = new SP_Sale();
        private SaleEntry saleEntry = new SaleEntry();

        // GET: SaleReturn
        public ActionResult FindSale()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblCustomerInvoice invoice;

            if (Session["SaleInvoiceNo"] != null)
            {
                var invoiceNo = Convert.ToString(Session["SaleInvoiceNo"]);
                if (!string.IsNullOrEmpty(invoiceNo))
                {
                    invoice = db.tblCustomerInvoice.Where(p => p.InvoiceNo == invoiceNo.Trim()).FirstOrDefault();
                }
                else
                {
                    invoice = db.tblCustomerInvoice.Find(0);
                }
            }
            else
            {
                invoice = db.tblCustomerInvoice.Find(0);
            }

            return View(invoice);
        }

        [HttpPost]
        public ActionResult FindSale(string invoiceID)
        {
            Session["SaleInvoiceNo"] = string.Empty;
            Session["SaleReturnMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var saleInvoice = db.tblCustomerInvoice.Where(p => p.InvoiceNo == invoiceID).FirstOrDefault();
            return View(saleInvoice);
        }

        [HttpPost]
        public ActionResult ReturnConfirm(FormCollection collection)
        {
            Session["SaleInvoiceNo"] = string.Empty;
            Session["SaleReturnMessage"] = string.Empty;
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

            int customerID = 0;
            int CustomerInvoiceID = 0;
            bool IsPayment = false;
            List<int> ProductIDs = new List<int>();
            List<int> ReturnQty = new List<int>();
            string[] keys = collection.AllKeys;
            foreach (var name in keys)
            {
                if (name.Contains("ProductID "))
                {
                    string idName = name;
                    string[] valueIDs = idName.Split(' ');
                    ProductIDs.Add(Convert.ToInt32(valueIDs[1]));
                    ReturnQty.Add(Convert.ToInt32(collection[idName].Split(',')[0]));
                }
            }
            string Description = "Sale Return";
            string[] CustomerInvoiceIDs = collection["customerInvoiceID"].Split(',');
            if (CustomerInvoiceIDs != null)
            {
                if (CustomerInvoiceIDs[0] != null)
                {
                    CustomerInvoiceID = Convert.ToInt32(CustomerInvoiceIDs[0]);
                }
            }
            if (collection["IsPayment"] != null)
            {
                string[] isPaymentDirCet = collection["IsPayment"].Split(',');
                if (isPaymentDirCet[0] == "on")
                {
                    IsPayment = true;
                }
                else
                {
                    IsPayment = false;
                }
            }
            else
            {
                IsPayment = false;
            }

            double TotalAmount = 0;
            var saleDetails = db.tblCustomerInvoiceDetail.Where(pd => pd.CustomerInvoiceID == CustomerInvoiceID).ToList();
            for (int i = 0; i < saleDetails.Count; i++)
            {
                foreach (var productID in ProductIDs)
                {
                    if (productID == saleDetails[i].ProductID)
                    {
                        TotalAmount += (ReturnQty[i] * saleDetails[i].SaleUnitPrice);
                    }
                }
            }

            var customerInvoice = db.tblCustomerInvoice.Find(CustomerInvoiceID);
            customerID = customerInvoice.CustomerID;
            if (TotalAmount == 0)
            {
                Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
                Session["SaleReturnMessage"] = "Must be at least One Product Return Qty";
                return RedirectToAction("FindSale");
            }

            string invoiceNo = "RIN" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
            var returnInvoiceHeader = new tblCustomerReturnInvoice()
            {
                BranchID = branchID,
                CompanyID = companyID,
                Description = Description,
                InvoiceDate = DateTime.Now,
                InvoiceNo = invoiceNo,
                CustomerID = customerID,
                UserID = userID,
                TotalAmount = TotalAmount,
                CustomerInvoiceID = CustomerInvoiceID
            };
            db.tblCustomerReturnInvoice.Add(returnInvoiceHeader);
            db.SaveChanges();

            var customer = db.tblCustomer.Find(customerID);
            string Message = saleEntry.ReturnSale(companyID, branchID, userID, invoiceNo, returnInvoiceHeader.CustomerInvoiceID.ToString(), returnInvoiceHeader.CustomerReturnInvoiceID, (float)TotalAmount, customerID.ToString(), customer.Customername, IsPayment);
            if (Message.Contains("Success"))
            {
                for (int i = 0; i < saleDetails.Count; i++)
                {
                    foreach (var productID in ProductIDs)
                    {
                        if (productID == saleDetails[i].ProductID)
                        {
                            if (ReturnQty[i] > 0)
                            {
                                var returnProductDetails = new tblCustomerReturnInvoiceDetail()
                                {
                                    CustomerInvoiceID = CustomerInvoiceID,
                                    SaleReturnQuantity = ReturnQty[i],
                                    ProductID = productID,
                                    SaleReturnUnitPrice = saleDetails[i].SaleUnitPrice,
                                    CustomerReturnInvoiceID = returnInvoiceHeader.CustomerReturnInvoiceID,
                                    CustomerInvoiceDetailID = saleDetails[i].CustomerInvoiceDetailID
                                };
                                db.tblCustomerReturnInvoiceDetail.Add(returnProductDetails);
                                db.SaveChanges();

                                var stock = db.tblStock.Find(productID);
                                stock.Quantity += ReturnQty[i];
                                db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }

                Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
                Session["SaleReturnMessage"] = "Return Successfully";
                return RedirectToAction("FindSale");
            }

            Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
            Session["SaleReturnMessage"] = "Some Unexpected Issue is Occured. Please Contact to Administrator";
            return RedirectToAction("FindSale");
        }
    }
}