using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseReturnController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly PurchaseEntry _purchaseEntry;

        public PurchaseReturnController(CloudDBEntities db, PurchaseEntry purchaseEntry)
        {
            _db = db;
            _purchaseEntry = purchaseEntry;
        }

        // GET: PurchaseReturn
        public ActionResult FindPurchase()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                tblSupplierInvoice invoice;

                if (Session["InvoiceNo"] != null)
                {
                    var invoiceNo = Convert.ToString(Session["InvoiceNo"]);
                    if (!string.IsNullOrEmpty(invoiceNo))
                    {
                        invoice = _db.tblSupplierInvoice
                                      .Where(p => p.InvoiceNo == invoiceNo.Trim())
                                      .FirstOrDefault();
                    }
                    else
                    {
                        invoice = _db.tblSupplierInvoice.Find(0);
                    }
                }
                else
                {
                    invoice = _db.tblSupplierInvoice.Find(0);
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
        public ActionResult FindPurchase(string invoiceID)
        {
            try
            {
                Session["InvoiceNo"] = string.Empty;
                Session["ReturnMessage"] = string.Empty;

                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var purchaseInvoice = _db.tblSupplierInvoice
                                         .Where(p => p.InvoiceNo == invoiceID)
                                         .FirstOrDefault();

                return View(purchaseInvoice);
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
                Session["InvoiceNo"] = string.Empty;
                Session["ReturnMessage"] = string.Empty;

                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                int supplierID = 0;
                int SupplierInvoiceID = 0;
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

                string Description = Resources.Messages.PurchaseReturn;
                string[] SupplierInvoiceIDs = collection["supplierInvoiceID"].Split(',');

                if (SupplierInvoiceIDs != null)
                {
                    if (SupplierInvoiceIDs[0] != null)
                    {
                        SupplierInvoiceID = Convert.ToInt32(SupplierInvoiceIDs[0]);
                    }
                }

                if (collection["IsPayment"] != null)
                {
                    string[] isPaymentDirCet = collection["IsPayment"].Split(',');
                    IsPayment = isPaymentDirCet[0] == "on";
                }

                double TotalAmount = 0;
                var purchaseDetails = _db.tblSupplierInvoiceDetail
                                         .Where(pd => pd.SupplierInvoiceID == SupplierInvoiceID)
                                         .ToList();
                for (int i = 0; i < purchaseDetails.Count; i++)
                {
                    foreach (var productID in ProductIDs)
                    {
                        if (productID == purchaseDetails[i].ProductID)
                        {
                            TotalAmount += (ReturnQty[i] * purchaseDetails[i].PurchaseUnitPrice);
                        }
                    }
                }

                var supplierInvoice = _db.tblSupplierInvoice.Find(SupplierInvoiceID);
                supplierID = supplierInvoice.SupplierID;
                if (TotalAmount == 0)
                {
                    Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
                    Session["ReturnMessage"] = Resources.Messages.OneProductReturnQtyError;
                    return RedirectToAction("FindPurchase");
                }

                string invoiceNo = "RPU" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var returnInvoiceHeader = new tblSupplierReturnInvoice()
                {
                    BranchID = branchID,
                    CompanyID = companyID,
                    Description = Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    SupplierID = supplierID,
                    UserID = userID,
                    TotalAmount = TotalAmount,
                    SupplierInvoiceID = SupplierInvoiceID
                };
                _db.tblSupplierReturnInvoice.Add(returnInvoiceHeader);
                _db.SaveChanges();

                var supplier = _db.tblSupplier.Find(supplierID);
                string Message = _purchaseEntry.ReturnPurchase(companyID, branchID, userID, invoiceNo, returnInvoiceHeader.SupplierInvoiceID.ToString(), returnInvoiceHeader.SupplierReturnInvoiceID, (float)TotalAmount, supplierID.ToString(), supplier.SupplierName, IsPayment);

                if (Message.Contains("Success"))
                {
                    for (int i = 0; i < purchaseDetails.Count; i++)
                    {
                        foreach (var productID in ProductIDs)
                        {
                            if (productID == purchaseDetails[i].ProductID)
                            {
                                if (ReturnQty[i] > 0)
                                {
                                    var returnProductDetails = new tblSupplierReturnInvoiceDetail()
                                    {
                                        SupplierInvoiceID = SupplierInvoiceID,
                                        PurchaseReturnQuantity = ReturnQty[i],
                                        ProductID = productID,
                                        PurchaseReturnUnitPrice = purchaseDetails[i].PurchaseUnitPrice,
                                        SupplierReturnInvoiceID = returnInvoiceHeader.SupplierReturnInvoiceID,
                                        SupplierInvoiceDetailID = purchaseDetails[i].SupplierInvoiceDetailID
                                    };
                                    _db.tblSupplierReturnInvoiceDetail.Add(returnProductDetails);
                                    _db.SaveChanges();

                                    var stock = _db.tblStock.Find(productID);
                                    if (stock != null)
                                    {
                                        stock.Quantity -= ReturnQty[i];

                                        _db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                                        _db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }

                    Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
                    Session["ReturnMessage"] = Resources.Messages.ReturnSuccessfully;

                    return RedirectToAction("FindPurchase");
                }

                Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
                Session["ReturnMessage"] = Resources.Messages.UnexpectedIssue;

                return RedirectToAction("FindPurchase");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}