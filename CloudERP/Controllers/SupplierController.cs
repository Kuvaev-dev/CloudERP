using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class SupplierController : Controller
    {
        private readonly CloudDBEntities _db;

        public SupplierController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: All Suppliers
        public ActionResult AllSuppliers()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                var tblSupplier = _db.tblSupplier.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser);
                return View(tblSupplier.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Supplier
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
                int userID = Convert.ToInt32(Session["UserID"]);
                var tblSupplier = _db.tblSupplier
                    .Include(t => t.tblBranch)
                    .Include(t => t.tblCompany)
                    .Include(t => t.tblUser)
                    .Where(s => s.BranchID == branchID && s.CompanyID == companyID);
                return View(tblSupplier.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Sub Branch Supplier
        public ActionResult SubBranchSupplier()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                List<BranchsSuppliersMV> branchsSuppliers = new List<BranchsSuppliersMV>();
                int branchID = Convert.ToInt32(Session["BranchID"]);
                List<int> branchIDs = BranchHelper.GetBranchsIDs(branchID, _db);

                foreach (var item in branchIDs)
                {
                    foreach (var supplier in _db.tblSupplier.Where(c => c.BranchID == item))
                    {
                        var newSupplier = new BranchsSuppliersMV
                        {
                            BranchName = supplier.tblBranch.BranchName,
                            CompanyName = supplier.tblCompany.Name,
                            SupplierName = supplier.SupplierName,
                            SupplierAddress = supplier.SupplierAddress,
                            SupplierConatctNo = supplier.SupplierConatctNo,
                            SupplierEmail = supplier.SupplierEmail,
                            Discription = supplier.Discription,
                            User = supplier.tblUser.UserName
                        };
                        branchsSuppliers.Add(newSupplier);
                    }
                }

                return View(branchsSuppliers);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Supplier/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblSupplier tblSupplier = _db.tblSupplier.Find(id);
                if (tblSupplier == null)
                {
                    return HttpNotFound();
                }
                return View(tblSupplier);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Supplier/Create
        public ActionResult Create()
        {
            return View(new tblSupplier());
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblSupplier tblSupplier)
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
                tblSupplier.CompanyID = companyID;
                tblSupplier.BranchID = branchID;
                tblSupplier.UserID = userID;
                if (ModelState.IsValid)
                {
                    var findSupplier = _db.tblSupplier.Where(s => s.SupplierName == tblSupplier.SupplierName
                                                           && s.SupplierConatctNo == tblSupplier.SupplierConatctNo
                                                           && s.BranchID == tblSupplier.BranchID).FirstOrDefault();
                    if (findSupplier == null)
                    {
                        _db.tblSupplier.Add(tblSupplier);
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Supplier already registered!";
                    }
                }

                return View(tblSupplier);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Supplier/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                tblSupplier tblSupplier = _db.tblSupplier.Find(id);
                if (tblSupplier == null)
                {
                    return HttpNotFound();
                }
                return View(tblSupplier);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblSupplier tblSupplier)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }
                int userID = Convert.ToInt32(Session["UserID"]);
                tblSupplier.UserID = userID;
                if (ModelState.IsValid)
                {
                    var findSupplier = _db.tblSupplier.Where(s => s.SupplierName == tblSupplier.SupplierName
                                                           && s.SupplierConatctNo == tblSupplier.SupplierConatctNo
                                                           && s.BranchID == tblSupplier.BranchID
                                                           && s.SupplierID != tblSupplier.SupplierID).FirstOrDefault();
                    if (findSupplier == null)
                    {
                        _db.Entry(tblSupplier).State = EntityState.Modified;
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Supplier already registered!";
                    }
                }

                return View(tblSupplier);
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