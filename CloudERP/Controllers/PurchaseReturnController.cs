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
        private readonly CloudDBEntities db = new CloudDBEntities();
        private readonly PurchaseEntry purchaseEntry = new PurchaseEntry();

        // GET: PurchaseReturn
        public ActionResult FindPurchase()
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
                    invoice = db.tblSupplierInvoice.Where(p => p.InvoiceNo == invoiceNo.Trim()).FirstOrDefault();
                }
                else
                {
                    invoice = db.tblSupplierInvoice.Find(0);
                }
            }
            else
            {
                invoice = db.tblSupplierInvoice.Find(0);
            }
            return View(invoice);
        }

        [HttpPost]
        public ActionResult FindPurchase(string invoiceID)
        {
            Session["InvoiceNo"] = string.Empty;
            Session["ReturnMessage"] = string.Empty;
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var purchaseInvoice = db.tblSupplierInvoice.Where(p => p.InvoiceNo == invoiceID).FirstOrDefault();
            return View(purchaseInvoice);
        }

        [HttpPost]
        public ActionResult ReturnConfirm(FormCollection collection)
        {
            Session["InvoiceNo"] = string.Empty;
            Session["ReturnMessage"] = string.Empty;
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
            string Description = "Purchase Return";
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
            var purchaseDetails = db.tblSupplierInvoiceDetail.Where(pd => pd.SupplierInvoiceID == SupplierInvoiceID).ToList();
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

            var supplierInvoice = db.tblSupplierInvoice.Find(SupplierInvoiceID);
            supplierID = supplierInvoice.SupplierID;
            if (TotalAmount == 0)
            {
                Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
                Session["ReturnMessage"] = "Must be at least One Product Return Qty";
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
            db.tblSupplierReturnInvoice.Add(returnInvoiceHeader);
            db.SaveChanges();

            var supplier = db.tblSupplier.Find(supplierID);
            string Message = purchaseEntry.ReturnPurchase(companyID, branchID, userID, invoiceNo, returnInvoiceHeader.SupplierInvoiceID.ToString(), returnInvoiceHeader.SupplierReturnInvoiceID, (float)TotalAmount, supplierID.ToString(), supplier.SupplierName, IsPayment);
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
                                db.tblSupplierReturnInvoiceDetail.Add(returnProductDetails);
                                db.SaveChanges();

                                var stock = db.tblStock.Find(productID);
                                stock.Quantity -= ReturnQty[i];
                                db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }

                Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
                Session["ReturnMessage"] = "Return Successfully";
                return RedirectToAction("FindPurchase");
            }

            Session["InvoiceNo"] = supplierInvoice.InvoiceNo;
            Session["ReturnMessage"] = "Some Unexpected Issue is Occured. Please Contact to Administrator";
            return RedirectToAction("FindPurchase");
        }
    }
}