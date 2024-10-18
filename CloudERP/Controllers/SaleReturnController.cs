using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SaleReturnController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SaleEntry _saleEntry;

        public SaleReturnController(CloudDBEntities db, SaleEntry saleEntry)
        {
            _db = db;
            _saleEntry = saleEntry;
        }

        // GET: SaleReturn
        public ActionResult FindSale()
        {
            try
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
                        invoice = _db.tblCustomerInvoice.Where(p => p.InvoiceNo == invoiceNo.Trim()).FirstOrDefault();
                    }
                    else
                    {
                        invoice = _db.tblCustomerInvoice.Find(0);
                    }
                }
                else
                {
                    invoice = _db.tblCustomerInvoice.Find(0);
                }

                return View(invoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult FindSale(string invoiceID)
        {
            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var saleInvoice = _db.tblCustomerInvoice.Where(p => p.InvoiceNo == invoiceID).FirstOrDefault();

                return View(saleInvoice);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult ReturnConfirm(FormCollection collection)
        {
            try
            {
                Session["SaleInvoiceNo"] = string.Empty;
                Session["SaleReturnMessage"] = string.Empty;

                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

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
                if (CustomerInvoiceIDs != null && CustomerInvoiceIDs.Length > 0)
                {
                    CustomerInvoiceID = Convert.ToInt32(CustomerInvoiceIDs[0]);
                }

                if (collection["IsPayment"] != null && collection["IsPayment"].Contains("on"))
                {
                    IsPayment = true;
                }

                double TotalAmount = 0;
                var saleDetails = _db.tblCustomerInvoiceDetail.Where(pd => pd.CustomerInvoiceID == CustomerInvoiceID).ToList();
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

                var customerInvoice = _db.tblCustomerInvoice.Find(CustomerInvoiceID);
                customerID = customerInvoice.CustomerID;

                if (TotalAmount == 0)
                {
                    Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
                    Session["SaleReturnMessage"] = Resources.Messages.OneProductReturnQtyError;
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

                _db.tblCustomerReturnInvoice.Add(returnInvoiceHeader);
                _db.SaveChanges();

                var customer = _db.tblCustomer.Find(customerID);
                string Message = _saleEntry.ReturnSale(companyID, branchID, userID, invoiceNo, returnInvoiceHeader.CustomerInvoiceID.ToString(), returnInvoiceHeader.CustomerReturnInvoiceID, (float)TotalAmount, customerID.ToString(), customer.Customername, IsPayment);

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

                                    _db.tblCustomerReturnInvoiceDetail.Add(returnProductDetails);
                                    _db.SaveChanges();

                                    var stock = _db.tblStock.Find(productID);
                                    stock.Quantity += ReturnQty[i];
                                    _db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                                    _db.SaveChanges();
                                }
                            }
                        }
                    }

                    Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
                    Session["SaleReturnMessage"] = Resources.Messages.ReturnSuccessfully;

                    return RedirectToAction("FindSale");
                }

                Session["SaleInvoiceNo"] = customerInvoice.InvoiceNo;
                Session["SaleReturnMessage"] = Resources.Messages.UnexpectedIssue;

                return RedirectToAction("FindSale");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}