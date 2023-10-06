using CloudERP.Models;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchaseCartController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: PurchaseCart
        public ActionResult NewPurchase()
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

            var findDetail = db.tblPurchaseCartDetail.Where(i => i.BranchID == branchID && i.CompanyID == companyID && i.UserID == userID);
            foreach (var item in findDetail)
            {
                totalAmount += item.PurchaseQuantity * item.PurchaseUnitPrice;
            }
            ViewBag.TotalAmount = totalAmount;

            return View(findDetail.ToList());
        }

        // POST: PurchaseCart
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

            var findDetail = db.tblPurchaseCartDetail.Where(i => i.ProductID == PID && i.BranchID == branchID && i.CompanyID == companyID).FirstOrDefault();
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
                    db.tblPurchaseCartDetail.Add(newItem);
                    db.SaveChanges();
                    ViewBag.Message = "Item Added Successfully!";
                }
            }
            else
            {
                ViewBag.Message = "Already Exist!";
            }

            return RedirectToAction("NewPurchase");
        }

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
                products.Add(new ProductMV() { Name = item.ProductName, ProductID = item.ProductID });
            }

            return Json(new { data = products }, JsonRequestBehavior.AllowGet);
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

            var product = db.tblPurchaseCartDetail.Find(id);
            if (product != null)
            {
                db.Entry(product).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
                ViewBag.Message = "Deleted Successfully.";
                return RedirectToAction("NewPurchase");
            }

            ViewBag.Message = "Some Unexptected issue is occure, please contact to concern person!";
            var find = db.tblPurchaseCartDetail.Where(i => i.BranchID == branchID && i.CompanyID == companyID && i.UserID == userID);
            return View(find.ToList());
        }

        public ActionResult CancelPurchase()
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

            var list = db.tblPurchaseCartDetail.Where(p => p.BranchID == branchID && p.CompanyID == companyID && p.UserID == userID).ToList();
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
                ViewBag.Message = "Purchase is Canceled.";
                return RedirectToAction("NewPurchase");
            }

            ViewBag.Message = "Some Unexptected issue is occure, please contact to concern person!";

            return RedirectToAction("NewPurchase");
        }

        public ActionResult SelectSupplier()
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
            var purchaseDetails = db.tblPurchaseCartDetail.Where(pd => pd.CompanyID == companyID && pd.BranchID == branchID).ToList();
            if (purchaseDetails.Count == 0)
            {
                ViewBag.Message = "Purchase Cart Empty";
                return View("NewPurchase");
            }
            var suppliers = db.tblSupplier.Where(s => s.CompanyID == companyID && s.BranchID == branchID).ToList();
            return View(suppliers);
        }

        [HttpPost]
        public ActionResult PurchaseConfirm(FormCollection collection)
        {
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            int supplierID = 0;
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

            var supplier = db.tblSupplier.Find(supplierID);
            var purchaseDetails = db.tblPurchaseCartDetail.Where(pd => pd.BranchID == branchID && pd.CompanyID == companyID).ToList();
            double totalAmount = 0;
            foreach (var item in purchaseDetails)
            {
                totalAmount += (item.PurchaseQuantity * item.PurchaseUnitPrice);
            }
            if (totalAmount == 0)
            {
                ViewBag.Message = "Purchase Cart Empty";
                return View("NewPurchase");
            }

            string invoiceNo = "PUR" + DateTime.Now.ToString("yyyyMMddHHmmss") + DateTime.Now.Millisecond;

            return View();
        }
    }
}