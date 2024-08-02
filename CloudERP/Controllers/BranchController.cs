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

            try
            {
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving branches: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult SubBranchs()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            try
            {
                var branches = _db.tblBranch.Include(t => t.tblBranchType)
                                            .Where(c => c.CompanyID == companyID && c.BrchID == branchID);

                return View(branches.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving sub-branches: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                tblBranch branch = _db.tblBranch.Find(id);
                if (branch == null)
                {
                    return HttpNotFound();
                }

                return View(branch);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving branch details: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Branch/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);

            try
            {
                ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName");
                ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType");

                return View(new tblBranch());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while preparing the create view: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                if (ModelState.IsValid)
                {
                    bool branchExists = _db.tblBranch.Any(b =>
                        b.CompanyID == companyID &&
                        (b.BranchName == tblBranch.BranchName ||
                         b.BranchContact == tblBranch.BranchContact ||
                         b.BranchAddress == tblBranch.BranchAddress)
                    );

                    if (branchExists)
                    {
                        ModelState.AddModelError("", "A branch with the same name, contact, or address already exists for this company.");
                        ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName");
                        ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);
                        return View(tblBranch);
                    }

                    _db.tblBranch.Add(tblBranch);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName");
                ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);

                return View(tblBranch);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while creating the branch: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving branch details for editing: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                if (ModelState.IsValid)
                {
                    bool branchExists = _db.tblBranch.Any(b =>
                        b.CompanyID == companyID &&
                        (b.BranchName == tblBranch.BranchName ||
                         b.BranchContact == tblBranch.BranchContact ||
                         b.BranchAddress == tblBranch.BranchAddress) &&
                        b.BranchID != tblBranch.BranchID
                    );

                    if (branchExists)
                    {
                        ModelState.AddModelError("", "A branch with the same name, contact, or address already exists for this company.");
                        ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName", tblBranch.BranchID);
                        ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);
                        return View(tblBranch);
                    }

                    _db.Entry(tblBranch).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.BrchID = new SelectList(_db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName", tblBranch.BranchID);
                ViewBag.BranchTypeID = new SelectList(_db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);

                return View(tblBranch);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while editing the branch: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                tblBranch branch = _db.tblBranch.Find(id);
                if (branch == null)
                {
                    return HttpNotFound();
                }

                return View(branch);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving branch details for deletion: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                tblBranch branch = _db.tblBranch.Find(id);
                _db.tblBranch.Remove(branch);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the branch: " + ex.Message;
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