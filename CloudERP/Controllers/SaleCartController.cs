using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CloudERP.Models;

namespace CloudERP.Controllers
{
    public class SaleCartController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SaleEntry saleEntry = new SaleEntry();

        public SaleCartController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: SaleCart/NewSale
        public ActionResult NewSale()
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

                var findDetail = _db.tblSaleCartDetail
                                    .Where(i => i.BranchID == branchID && i.CompanyID == companyID && i.UserID == userID)
                                    .ToList();

                double totalAmount = findDetail.Sum(item => item.SaleQuantity * item.SaleUnitPrice);
                ViewBag.TotalAmount = totalAmount;

                return View(findDetail);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/AddItem
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

                var checkQty = _db.tblStock.Find(PID);
                if (Qty > checkQty.Quantity)
                {
                    ViewBag.Message = "Sale Quantity must be less than or equal to available quantity.";
                    return RedirectToAction("NewSale");
                }

                var findDetail = _db.tblSaleCartDetail
                                    .FirstOrDefault(i => i.ProductID == PID && i.BranchID == branchID && i.CompanyID == companyID);

                if (findDetail == null)
                {
                    if (PID > 0 && Qty > 0 && Price > 0)
                    {
                        var newItem = new tblSaleCartDetail()
                        {
                            ProductID = PID,
                            SaleQuantity = Qty,
                            SaleUnitPrice = Price,
                            BranchID = branchID,
                            CompanyID = companyID,
                            UserID = userID
                        };

                        _db.tblSaleCartDetail.Add(newItem);
                        _db.SaveChanges();
                        ViewBag.Message = "Item added successfully!";
                    }
                }
                else
                {
                    ViewBag.Message = "Item already exists!";
                }

                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/DeleteConfirm/5
        [HttpPost]
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

                var product = _db.tblSaleCartDetail.Find(id);
                if (product != null)
                {
                    _db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                    _db.SaveChanges();
                    ViewBag.Message = "Deleted successfully.";
                    return RedirectToAction("NewSale");
                }

                ViewBag.Message = "Product not found.";
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/GetProduct
        [HttpPost]
        public ActionResult GetProduct()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                List<ProductMV> products = _db.tblStock
                                            .Where(p => p.BranchID == branchID && p.CompanyID == companyID)
                                            .Select(item => new ProductMV()
                                            {
                                                Name = item.ProductName + " (Avl Qty: " + item.Quantity + ")",
                                                ProductID = item.ProductID
                                            })
                                            .ToList();

                return Json(new { data = products }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return Json(new { error = "An error occurred while retrieving products." });
            }
        }

        // POST: SaleCart/GetProductDetails/5
        [HttpPost]
        public ActionResult GetProductDetails(int? id)
        {
            try
            {
                var product = _db.tblStock.Find(id);
                if (product != null)
                {
                    return Json(new { data = product.SaleUnitPrice }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { error = "Product not found." });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return Json(new { error = "An error occurred while retrieving product details." });
            }
        }

        // POST: SaleCart/CancelSale
        [HttpPost]
        public ActionResult CancelSale()
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

                var saleDetails = _db.tblSaleCartDetail
                                    .Where(pd => pd.BranchID == branchID && pd.CompanyID == companyID && pd.UserID == userID)
                                    .ToList();

                foreach (var item in saleDetails)
                {
                    _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                }

                _db.SaveChanges();

                ViewBag.Message = "Sale canceled successfully.";
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: SaleCart/SelectCustomer
        public ActionResult SelectCustomer()
        {
            try
            {
                Session["ErrorMessageSale"] = string.Empty;

                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var saleDetails = _db.tblSaleCartDetail.FirstOrDefault(pd => pd.CompanyID == companyID && pd.BranchID == branchID);
                if (saleDetails == null)
                {
                    Session["ErrorMessageSale"] = "Sale cart is empty.";
                    return RedirectToAction("NewSale");
                }

                var customers = _db.tblCustomer
                                    .Where(s => s.CompanyID == companyID && s.BranchID == branchID)
                                    .ToList();

                return View(customers);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: SaleCart/SaleConfirm
        [HttpPost]
        public ActionResult SaleConfirm(FormCollection collection)
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

                int customerID = 0;
                bool IsPayment = false;
                string[] keys = collection.AllKeys;
                foreach (var name in keys)
                {
                    if (name.Contains("name"))
                    {
                        string idName = name;
                        string[] valueIDs = idName.Split(' ');
                        customerID = Convert.ToInt32(valueIDs[1]);
                    }
                }

                string Description = collection["item.Description"];

                if (collection["IsPayment"] != null)
                {
                    IsPayment = collection["IsPayment"].Contains("on");
                }

                var customer = _db.tblCustomer.Find(customerID);
                var saleDetails = _db.tblSaleCartDetail
                                    .Where(pd => pd.BranchID == branchID && pd.CompanyID == companyID)
                                    .ToList();

                double totalAmount = saleDetails.Sum(item => item.SaleQuantity * item.SaleUnitPrice);

                if (totalAmount == 0)
                {
                    ViewBag.Message = "Sale cart is empty.";
                    return RedirectToAction("NewSale");
                }

                string invoiceNo = "INV" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;
                var invoiceHeader = new tblCustomerInvoice()
                {
                    BranchID = branchID,
                    Title = "Sale Invoice " + customer.Customername,
                    CompanyID = companyID,
                    Description = Description,
                    InvoiceDate = DateTime.Now,
                    InvoiceNo = invoiceNo,
                    CustomerID = customerID,
                    UserID = userID,
                    TotalAmount = totalAmount
                };

                _db.tblCustomerInvoice.Add(invoiceHeader);
                _db.SaveChanges();

                foreach (var item in saleDetails)
                {
                    var newSaleDetails = new tblCustomerInvoiceDetail()
                    {
                        ProductID = item.ProductID,
                        SaleQuantity = item.SaleQuantity,
                        SaleUnitPrice = item.SaleUnitPrice,
                        CustomerInvoiceID = invoiceHeader.CustomerInvoiceID
                    };

                    _db.tblCustomerInvoiceDetail.Add(newSaleDetails);
                }

                _db.SaveChanges();

                string Message = saleEntry.ConfirmSale(companyID, branchID, userID, invoiceNo, invoiceHeader.CustomerInvoiceID.ToString(), (float)totalAmount, customerID.ToString(), customer.Customername, IsPayment);
                if (Message.Contains("Success"))
                {
                    foreach (var item in saleDetails)
                    {
                        var stockItem = _db.tblStock.Find(item.ProductID);
                        if (stockItem != null)
                        {
                            stockItem.Quantity += item.SaleQuantity;
                            _db.Entry(stockItem).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                        _db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    }
                    _db.SaveChanges();
                }

                if (Message.Contains("Success"))
                {
                    return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = invoiceHeader.CustomerInvoiceID });
                }

                Session["Message"] = Message;
                return RedirectToAction("NewSale");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}