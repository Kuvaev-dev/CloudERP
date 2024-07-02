using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CloudDBEntities _db;

        public CustomerController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: All Customers
        public ActionResult AllCustomers()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                var tblCustomer = _db.tblCustomer.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser);
                return View(tblCustomer.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        // GET: Customer
        public ActionResult Index()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                var tblCustomer = _db.tblCustomer.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser)
                                                .Where(c => c.CompanyID == companyID && c.BranchID == branchID);
                return View(tblCustomer.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Sub Branch Customer
        public ActionResult SubBranchCustomer()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                List<BranchsCustomersMV> branchsCustomers = new List<BranchsCustomersMV>();
                int branchID = Convert.ToInt32(Session["BranchID"]);
                List<int> branchIDs = BranchHelper.GetBranchsIDs(branchID, _db);

                foreach (var item in branchIDs)
                {
                    foreach (var customer in _db.tblCustomer.Where(c => c.BranchID == item))
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Customer/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblCustomer tblCustomer = _db.tblCustomer.Find(id);
                if (tblCustomer == null)
                {
                    return HttpNotFound();
                }
                return View(tblCustomer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult CustomerDetails(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblCustomer tblCustomer = _db.tblCustomer.Find(id);
                if (tblCustomer == null)
                {
                    return HttpNotFound();
                }
                return View(tblCustomer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Customer/Create
        public ActionResult Create()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                return View(new tblCustomer());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Customer/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblCustomer tblCustomer)
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
                tblCustomer.CompanyID = companyID;
                tblCustomer.BranchID = branchID;
                tblCustomer.UserID = userID;
                if (ModelState.IsValid)
                {
                    var findCustomer = _db.tblCustomer.Where(c => c.Customername == tblCustomer.Customername
                                                              && c.CustomerContact == tblCustomer.CustomerContact
                                                              && c.BranchID == branchID).FirstOrDefault();
                    if (findCustomer == null)
                    {
                        _db.tblCustomer.Add(tblCustomer);
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Already Exist!";
                    }
                }

                return View(tblCustomer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Customer/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblCustomer tblCustomer = _db.tblCustomer.Find(id);
                if (tblCustomer == null)
                {
                    return HttpNotFound();
                }

                return View(tblCustomer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Customer/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCustomer tblCustomer)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                int userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
                tblCustomer.UserID = userID;
                if (ModelState.IsValid)
                {
                    var findCustomer = _db.tblCustomer.Where(c => c.Customername == tblCustomer.Customername
                                                              && c.CustomerContact == tblCustomer.CustomerContact
                                                              && c.CustomerID != tblCustomer.CustomerID).FirstOrDefault();
                    if (findCustomer == null)
                    {
                        _db.Entry(tblCustomer).State = EntityState.Modified;
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Already Exist!";
                    }
                }

                return View(tblCustomer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Customer/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblCustomer tblCustomer = _db.tblCustomer.Find(id);
                if (tblCustomer == null)
                {
                    return HttpNotFound();
                }
                return View(tblCustomer);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                tblCustomer tblCustomer = _db.tblCustomer.Find(id);
                _db.tblCustomer.Remove(tblCustomer);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}