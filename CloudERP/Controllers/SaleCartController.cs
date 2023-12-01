using DatabaseAccess.Code;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CloudERP.Models;

namespace CloudERP.Controllers
{
    public class SaleCartController : Controller
    {
        private readonly CloudDBEntities db = new CloudDBEntities();
        private readonly SaleEntry saleEntry = new SaleEntry();

        // GET: SaleCart
        public ActionResult NewSale()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            double totalAmount = 0;
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var findDetail = db.tblSaleCartDetail.Where(i => i.BranchID == branchID && i.CompanyID == companyID && i.UserID == userID);
            foreach (var item in findDetail)
            {
                totalAmount += item.SaleQuantity * item.SaleUnitPrice;
            }
            ViewBag.TotalAmount = totalAmount;

            return View(findDetail.ToList());
        }

        [HttpPost]
        public ActionResult AddItem(int PID, int Qty, float Price)
        {
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

            var checkQty = db.tblStock.Find(PID);
            if (Qty > checkQty.Quantity)
            {
                ViewBag.Message = "Sale Quantity Must be Less Ther or Equal to Avl Qty";
                return RedirectToAction("NewSale");
            }

            var findDetail = db.tblSaleCartDetail.Where(i => i.ProductID == PID && i.BranchID == branchID && i.CompanyID == companyID).FirstOrDefault();
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

                    db.tblSaleCartDetail.Add(newItem);
                    db.SaveChanges();
                    ViewBag.Message = "Item Added Successfully!";
                }
            }
            else
            {
                ViewBag.Message = "Already Exist!";
            }

            return RedirectToAction("NewSale");
        }

        public ActionResult DeleteConfirm(int? id)
        {
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

            var product = db.tblSaleCartDetail.Find(id);
            if (product != null)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ViewBag.Message = "Deleted Successfully.";
                return RedirectToAction("NewSale");
            }

            ViewBag.Message = "Some Unexptected issue is occure, please contact to concern person!";
            var find = db.tblSaleCartDetail.Where(i => i.BranchID == branchID && i.CompanyID == companyID && i.UserID == userID);
            return View(find.ToList());
        }

        [HttpPost]
        public ActionResult GetProduct()
        {
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

            List<ProductMV> products = new List<ProductMV>();
            var productList = db.tblStock.Where(p => p.BranchID == branchID && p.CompanyID == companyID).ToList();
            foreach (var item in productList)
            {
                products.Add(new ProductMV() { Name = item.ProductName + " (Avl Qty: " + item.Quantity + ")", ProductID = item.ProductID });
            }

            return Json(new { data = products }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetProductDetails(int? id)
        {
            var product = db.tblStock.Find(id);
            return Json(new { data = product.SaleUnitPrice }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CancelSale()
        {
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

            var list = db.tblSaleCartDetail.Where(p => p.BranchID == branchID && p.CompanyID == companyID && p.UserID == userID).ToList();
            bool cancelstatus = false;

            foreach (var item in list)
            {
                db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                int noofrecords = db.SaveChanges();
                if (cancelstatus == false)
                {
                    if (noofrecords > 0)
                    {
                        cancelstatus = true;
                    }
                }
            }

            if (cancelstatus == true)
            {
                ViewBag.Message = "Sale is Canceled.";
                return RedirectToAction("NewSale");
            }

            ViewBag.Message = "Some Unexptected issue is occure, please contact to concern person!";

            return RedirectToAction("NewSale");
        }

        public ActionResult SelectCustomer()
        {
            Session["ErrorMessageSale"] = string.Empty;
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
            var saleDetails = db.tblSaleCartDetail.Where(pd => pd.CompanyID == companyID && pd.BranchID == branchID).FirstOrDefault();
            if (saleDetails == null)
            {
                Session["ErrorMessageSale"] = "Sale Cart Empty";
                return RedirectToAction("NewSale");
            }
            var customers = db.tblCustomer.Where(s => s.CompanyID == companyID && s.BranchID == branchID).ToList();
            return View(customers);
        }

        [HttpPost]
        public ActionResult SaleConfirm(FormCollection collection)
        {
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));

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
            string Description = string.Empty;
            string[] DescriptionList = collection["item.Description"].Split(',');
            if (DescriptionList != null)
            {
                if (DescriptionList[0] != null)
                {
                    Description = DescriptionList[0];
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
            var customer = db.tblCustomer.Find(customerID);
            var saleDetails = db.tblSaleCartDetail.Where(pd => pd.BranchID == branchID && pd.CompanyID == companyID).ToList();
            double totalAmount = 0;
            foreach (var item in saleDetails)
            {
                totalAmount += (item.SaleQuantity * item.SaleUnitPrice);
            }
            if (totalAmount == 0)
            {
                ViewBag.Message = "Sale Cart Empty";
                return View("NewSale");
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
            db.tblCustomerInvoice.Add(invoiceHeader);
            db.SaveChanges();

            foreach (var item in saleDetails)
            {
                var newSaleDetails = new tblCustomerInvoiceDetail()
                {
                    ProductID = item.ProductID,
                    SaleQuantity = item.SaleQuantity,
                    SaleUnitPrice = item.SaleUnitPrice,
                    CustomerInvoiceID = invoiceHeader.CustomerInvoiceID
                };
                db.tblCustomerInvoiceDetail.Add(newSaleDetails);
                db.SaveChanges();
            }

            string Message = saleEntry.ConfirmSale(companyID, branchID, userID, invoiceNo, invoiceHeader.CustomerInvoiceID.ToString(), (float)totalAmount, customerID.ToString(), customer.Customername, IsPayment);
            if (Message.Contains("Success"))
            {
                foreach (var item in saleDetails)
                {
                    var stockItem = db.tblStock.Find(item.ProductID);
                    stockItem.Quantity += item.SaleQuantity;
                    db.Entry(stockItem).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    db.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }

            if (Message.Contains("Success"))
            {
                return RedirectToAction("PrintSaleInvoice", "SalePayment", new { id = invoiceHeader.CustomerInvoiceID });
            }

            Session["Message"] = Message;

            return RedirectToAction("NewSale");
        }
    }
}