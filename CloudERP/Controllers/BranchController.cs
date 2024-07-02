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
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = GetCompanyID();
            int branchTypeID = GetBranchTypeID();
            int branchID = GetBranchID();

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
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = GetCompanyID();
            int branchID = GetBranchID();

            var branches = _db.tblBranch.Include(t => t.tblBranchType)
                                        .Where(c => c.CompanyID == companyID && c.BrchID == branchID);

            return View(branches.ToList());
        }

        // GET: Branch/Details/5
        public ActionResult Details(int? id)
        {
            if (!IsUserLoggedIn())
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
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = GetCompanyID();
            ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName");
            ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType");

            return View(new tblBranch());
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblBranch tblBranch)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = GetCompanyID();
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
            if (!IsUserLoggedIn())
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

            int companyID = GetCompanyID();
            ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName", branch.BrchID);
            ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType", branch.BranchTypeID);

            return View(branch);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblBranch tblBranch)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = GetCompanyID();
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
            if (!IsUserLoggedIn())
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
            if (!IsUserLoggedIn())
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

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"]));
        }

        private int GetCompanyID()
        {
            return Convert.ToInt32(Session["CompanyID"]);
        }

        private int GetBranchTypeID()
        {
            int branchTypeID;
            int.TryParse(Convert.ToString(Session["BranchTypeID"]), out branchTypeID);
            return branchTypeID;
        }

        private int GetBranchID()
        {
            return Convert.ToInt32(Session["BrchID"]);
        }
    }
}