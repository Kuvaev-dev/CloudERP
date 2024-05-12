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
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var tblSupplier = _db.tblSupplier.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser);
            return View(tblSupplier.ToList());
        }

        // GET: Supplier
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
            var tblSupplier = _db.tblSupplier.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser)
                                            .Where(s => s.BranchID == branchID && s.CompanyID == companyID);
            return View(tblSupplier.ToList());
        }

        // GET: Sub Branch Supplier
        public ActionResult SubBranchSupplier()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            List<BranchsSuppliersMV> branchsSuppliers = new List<BranchsSuppliersMV>();
            int branchID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
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

        // GET: Supplier/Details/5
        public ActionResult Details(int? id)
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

        public ActionResult SupplierDetails(int? id)
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

        // GET: Supplier/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Supplier/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblSupplier tblSupplier)
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
                    ViewBag.Message = "Already Registered!";
                }
            }

            return View(tblSupplier);
        }

        // GET: Supplier/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Supplier/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblSupplier tblSupplier)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
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
                    ViewBag.Message = "Already Registered!";
                }
            }

            return View(tblSupplier);
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
