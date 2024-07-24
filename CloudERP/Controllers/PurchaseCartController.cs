using CloudERP.Models;
using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseCartController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly PurchaseEntry _purchaseEntry;

        public PurchaseCartController(CloudDBEntities db)
        {
            _db = db;
            _purchaseEntry = new PurchaseEntry(_db);
        }

        // GET: PurchaseCart/NewPurchase
        public ActionResult NewPurchase()
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

                var findDetail = _db.tblPurchaseCartDetail.Where(i => i.BranchID == branchID && i.CompanyID == companyID && i.UserID == userID).ToList();
                double totalAmount = findDetail.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);
                ViewBag.TotalAmount = totalAmount;

                return View(findDetail);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching the purchase details. Please try again later.";
                return View(new List<tblPurchaseCartDetail>());
            }
        }

        // POST: PurchaseCart/AddItem
        [HttpPost]
        public ActionResult AddItem(int PID, int Qty, float Price)
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

                var findDetail = _db.tblPurchaseCartDetail.FirstOrDefault(i => i.ProductID == PID && i.BranchID == branchID && i.CompanyID == companyID);
                if (findDetail == null)
                {
                    if (PID > 0 && Qty > 0 && Price > 0)
                    {
                        var newItem = new tblPurchaseCartDetail()
                        {
                            ProductID = PID,
                            PurchaseQuantity = Qty,
                            PurchaseUnitPrice = Price,
                            BranchID = branchID,
                            CompanyID = companyID,
                            UserID = userID
                        };

                        _db.tblPurchaseCartDetail.Add(newItem);
                        _db.SaveChanges();
                        ViewBag.Message = "Item Added Successfully!";
                    }
                }
                else
                {
                    ViewBag.Message = "Item already exists!";
                }

                return RedirectToAction("NewPurchase");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An error occurred while adding the item to the purchase cart. Please try again later.";
                return RedirectToAction("NewPurchase");
            }
        }

        // POST: PurchaseCart/GetProduct
        [HttpPost]
        public ActionResult GetProduct()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return Json(new { data = new List<ProductMV>() }, JsonRequestBehavior.AllowGet);
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                List<ProductMV> products = _db.tblStock.Where(p => p.BranchID == branchID && p.CompanyID == companyID)
                                                        .Select(item => new ProductMV { Name = item.ProductName, ProductID = item.ProductID })
                                                        .ToList();

                return Json(new { data = products }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return Json(new { data = new List<ProductMV>() }, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: PurchaseCart/DeleteConfirm
        public ActionResult DeleteConfirm(int? id)
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

                var product = _db.tblPurchaseCartDetail.Find(id);
                if (product != null)
                {
                    _db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                    _db.SaveChanges();
                    ViewBag.Message = "Deleted Successfully.";
                    return RedirectToAction("NewPurchase");
                }

                ViewBag.Message = "Some unexpected issue occurred. Please contact the concerned person!";
                var find = _db.tblPurchaseCartDetail.Where(i => i.BranchID == branchID && i.CompanyID == companyID && i.UserID == userID).ToList();
                
                return View(find);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the item from the purchase cart. Please try again later.";
                return RedirectToAction("NewPurchase");
            }
        }

        // POST: PurchaseCart/CancelPurchase
        [HttpPost]
        public ActionResult CancelPurchase()
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

                var list = _db.tblPurchaseCartDetail.Where(p => p.BranchID == branchID && p.CompanyID == companyID && p.UserID == userID).ToList();
                bool cancelstatus = false;

                foreach (var item in list)
                {
                    _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    int noofrecords = _db.SaveChanges();
                    if (cancelstatus == false && noofrecords > 0)
                    {
                        cancelstatus = true;
                    }
                }

                if (cancelstatus)
                {
                    ViewBag.Message = "Purchase is canceled.";
                }
                else
                {
                    ViewBag.Message = "Some unexpected issue occurred. Please contact the concerned person!";
                }

                return RedirectToAction("NewPurchase");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An error occurred while canceling the purchase. Please try again later.";
                return RedirectToAction("NewPurchase");
            }
        }

        // GET: PurchaseCart/SelectSupplier
        public ActionResult SelectSupplier()
        {
            try
            {
                Session["ErrorMessagePurchase"] = string.Empty;

                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var checkPurchaseCart = _db.tblPurchaseCartDetail.FirstOrDefault(pd => pd.BranchID == branchID && pd.CompanyID == companyID);
                if (checkPurchaseCart == null)
                {
                    Session["ErrorMessagePurchase"] = "Purchase Cart is empty";
                    return RedirectToAction("NewPurchase");
                }

                var suppliers = _db.tblSupplier.Where(s => s.CompanyID == companyID && s.BranchID == branchID).ToList();
                
                return View(suppliers);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An error occurred while selecting the supplier. Please try again later.";
                return RedirectToAction("NewPurchase");
            }
        }

        // POST: PurchaseCart/PurchaseConfirm
        [HttpPost]
        public ActionResult PurchaseConfirm(FormCollection collection)
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

                int supplierID = 0;
                bool IsPayment = false;
                string[] keys = collection.AllKeys;
                foreach (var name in keys)
                {
                    if (name.Contains("name"))
                    {
                        string idName = name;
                        string[] valueIDs = idName.Split(' ');
                        supplierID = Convert.ToInt32(valueIDs[1]);
                    }
                }

                string Description = string.Empty;
                string[] DescriptionList = collection["item.Description"].Split(',');
                if (DescriptionList != null && DescriptionList.Length > 0)
                {
                    Description = DescriptionList[0];
                }

                if (collection["IsPayment"] != null)
                {
                    string[] isPaymentDirCet = collection["IsPayment"].Split(',');
                    if (isPaymentDirCet[0] == "on")
                    {
                        IsPayment = true;
                    }
                }

                var supplier = _db.tblSupplier.Find(supplierID);
                var purchaseDetails = _db.tblPurchaseCartDetail.Where(pd => pd.BranchID == branchID && pd.CompanyID == companyID).ToList();
                double totalAmount = purchaseDetails.Sum(item => item.PurchaseQuantity * item.PurchaseUnitPrice);

                if (totalAmount == 0)
                {
                    ViewBag.Message = "Purchase Cart is empty";
                    return RedirectToAction("NewPurchase");
                }

                string invoiceNo = "PUR" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var invoiceHeader = new tblSupplierInvoice()
                {
                    BranchID = branchID,
                    CompanyID = companyID,
                    Description = Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    SupplierID = supplierID,
                    UserID = userID,
                    TotalAmount = totalAmount
                };

                _db.tblSupplierInvoice.Add(invoiceHeader);
                _db.SaveChanges();

                foreach (var item in purchaseDetails)
                {
                    var newPurchaseDetails = new tblSupplierInvoiceDetail()
                    {
                        ProductID = item.ProductID,
                        PurchaseQuantity = item.PurchaseQuantity,
                        PurchaseUnitPrice = item.PurchaseUnitPrice,
                        SupplierInvoiceID = invoiceHeader.SupplierInvoiceID
                    };
                    _db.tblSupplierInvoiceDetail.Add(newPurchaseDetails);
                }

                _db.SaveChanges();

                string Message = _purchaseEntry.ConfirmPurchase(companyID, branchID, userID, invoiceNo, invoiceHeader.SupplierInvoiceID.ToString(), (float)totalAmount, supplierID.ToString(), supplier.SupplierName, IsPayment);

                if (Message.Contains("Success"))
                {
                    foreach (var item in purchaseDetails)
                    {
                        var stockItem = _db.tblStock.Find(item.ProductID);
                        if (stockItem != null)
                        {
                            stockItem.CurrentPurchaseUnitPrice = item.PurchaseUnitPrice;
                            stockItem.Quantity += item.PurchaseQuantity;
                            _db.Entry(stockItem).State = System.Data.Entity.EntityState.Modified;
                        }
                        _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                    _db.SaveChanges();
                    return RedirectToAction("PrintPurchaseInvoice", "PurchasePayment", new { id = invoiceHeader.SupplierInvoiceID });
                }

                Session["Message"] = Message;
                
                return RedirectToAction("NewPurchase");
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "An error occurred while confirming the purchase. Please try again later.";
                return RedirectToAction("NewPurchase");
            }
        }
    }
}