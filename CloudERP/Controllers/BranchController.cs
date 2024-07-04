using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class BranchController : Controller
    {
        private readonly CloudDBEntities _db;

        public BranchController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: Branch
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchTypeID = Convert.ToInt32(Session["BranchTypeID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            IQueryable<tblBranch> branches;

            if (branchTypeID == 1) // Main Branch
            {
                branches = _db.tblBranch.Include(t => t.tblBranchType).Where(c => c.CompanyID == companyID);
            }
            else
            {
                branches = _db.tblBranch.Include(t => t.tblBranchType).Where(c => c.BrchID == branchID);
            }

            return View(branches.ToList());
        }

        public ActionResult SubBranchs()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            var branches = _db.tblBranch.Include(t => t.tblBranchType)
                                        .Where(c => c.CompanyID == companyID && c.BrchID == branchID);

            return View(branches.ToList());
        }

        // GET: Branch/Details/5
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

            tblBranch branch = _db.tblBranch.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }

            return View(branch);
        }

        // GET: Branch/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);

            ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName");
            ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType");

            return View(new tblBranch());
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblBranch tblBranch)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            tblBranch.CompanyID = companyID;

            if (ModelState.IsValid)
            {
                _db.tblBranch.Add(tblBranch);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName");
            ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);

            return View(tblBranch);
        }

        // GET: Branch/Edit/5
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

            tblBranch branch = _db.tblBranch.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);

            ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName", branch.BrchID);
            ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType", branch.BranchTypeID);

            return View(branch);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblBranch tblBranch)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            tblBranch.CompanyID = companyID;

            if (ModelState.IsValid)
            {
                _db.Entry(tblBranch).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName", tblBranch.BrchID);
            ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);

            return View(tblBranch);
        }

        // GET: Branch/Delete/5
        public ActionResult Delete(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblBranch branch = _db.tblBranch.Find(id);
            if (branch == null)
            {
                return HttpNotFound();
            }

            return View(branch);
        }

        // POST: Branch/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            tblBranch branch = _db.tblBranch.Find(id);
            _db.tblBranch.Remove(branch);
            _db.SaveChanges();

            return RedirectToAction("Index");
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