using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudERP.Models;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class CustomerController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: All Customers
        public ActionResult AllCustomers()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var tblCustomer = db.tblCustomer.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser);
            return View(tblCustomer.ToList());
        }

        // GET: Customer
        public ActionResult Index()
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
            var tblCustomer = db.tblCustomer.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser)
                                            .Where(c => c.CompanyID == companyID && c.BranchID == branchID);
            return View(tblCustomer.ToList());
        }

        // GET: Sub Branch Customer
        public ActionResult SubBranchCustomer()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            List<int> branchIDs = new List<int>();
            List<int> isSubBranchsFirst = new List<int>();
            List<int> isSubBranchsSecond = new List<int>();
            List<BranchsCustomersMV> branchsCustomers = new List<BranchsCustomersMV>();
            
            int branchID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            var brnch = db.tblBranch.Where(b => b.BrchID == branchID);

            foreach (var item in brnch)
            {
                isSubBranchsFirst.Add(item.BranchID);
            }
        subBranch:
            foreach (var item in isSubBranchsFirst)
            {
                branchIDs.Add(item);
                foreach (var sub in db.tblBranch.Where(b => b.BrchID == item))
                {
                    isSubBranchsSecond.Add(sub.BranchID);
                }
            }
            if (isSubBranchsSecond.Count > 0)
            {
                isSubBranchsFirst.Clear();
                foreach (var item in isSubBranchsSecond)
                {
                    isSubBranchsFirst.Add(item);
                }
                isSubBranchsSecond.Clear();
                goto subBranch;
            }

            foreach (var item in branchIDs)
            {
                foreach (var customer in db.tblCustomer.Where(c => c.BranchID == item))
                {
                    var newCustomer = new BranchsCustomersMV
                    {
                        BranchName = customer.tblBranch.BranchName,
                        CompanyName = customer.tblCompany.Name,
                        CustomerAddress = customer.CustomerAddress,
                        CustomerArea = customer.CustomerArea,
                        CustomerContact = customer.CustomerContact,
                        Customername = customer.Customername,
                        Description = customer.Description,
                        User = customer.tblUser.UserName
                    };
                    branchsCustomers.Add(newCustomer);
                }
            }

            return View(branchsCustomers);
        }

        // GET: Customer/Details/5
        public ActionResult Details(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomer.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: Customer/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblCustomer tblCustomer)
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
            tblCustomer.CompanyID = companyID;
            tblCustomer.BranchID = branchID;
            tblCustomer.UserID = userID;
            if (ModelState.IsValid)
            {
                var findCustomer = db.tblCustomer.Where(c => c.Customername == tblCustomer.Customername && c.CustomerContact == tblCustomer.CustomerContact).FirstOrDefault();
                if (findCustomer == null)
                {
                    db.tblCustomer.Add(tblCustomer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            return View(tblCustomer);
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomer.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }

            return View(tblCustomer);
        }

        // POST: Customer/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCustomer tblCustomer)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblCustomer.UserID = userID;
            if (ModelState.IsValid)
            {
                var findCustomer = db.tblCustomer.Where(c => c.Customername == tblCustomer.Customername 
                                                          && c.CustomerContact == tblCustomer.CustomerContact
                                                          && c.CustomerID != tblCustomer.CustomerID).FirstOrDefault();
                if (findCustomer == null)
                {
                    db.Entry(tblCustomer).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            return View(tblCustomer);
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCustomer tblCustomer = db.tblCustomer.Find(id);
            if (tblCustomer == null)
            {
                return HttpNotFound();
            }
            return View(tblCustomer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblCustomer tblCustomer = db.tblCustomer.Find(id);
            db.tblCustomer.Remove(tblCustomer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
