using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class AccountSubControlController : Controller
    {
        private readonly CloudDBEntities _db;

        public AccountSubControlController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: AccountSubControl
        public ActionResult Index()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            var tblAccountSubControl = _db.tblAccountSubControl
                .Include(t => t.tblAccountControl)
                .Include(t => t.tblAccountHead)
                .Include(t => t.tblBranch)
                .Include(t => t.tblUser)
                .Where(t => t.CompanyID == companyID && t.BranchID == branchID);

            return View(tblAccountSubControl.ToList());
        }

        // GET: AccountSubControl/Create
        public ActionResult Create()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID), "AccountControlID", "AccountControlName", "0");
            return View(new tblAccountSubControl());
        }

        // POST: AccountSubControl/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountSubControl tblAccountSubControl)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int userID = Convert.ToInt32(Session["UserID"]);

            tblAccountSubControl.CompanyID = companyID;
            tblAccountSubControl.BranchID = branchID;
            tblAccountSubControl.UserID = userID;
            tblAccountSubControl.AccountHeadID = _db.tblAccountControl.Find(tblAccountSubControl.AccountControlID)?.AccountHeadID ?? 0;

            if (ModelState.IsValid)
            {
                try
                {
                    var findSubControl = _db.tblAccountSubControl.FirstOrDefault(s => s.CompanyID == tblAccountSubControl.CompanyID
                        && s.BranchID == tblAccountSubControl.BranchID
                        && s.AccountSubControlName == tblAccountSubControl.AccountSubControlName);

                    if (findSubControl == null)
                    {
                        _db.tblAccountSubControl.Add(tblAccountSubControl);
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Already Exist!";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                    return RedirectToAction("EP500", "EP");
                }
            }

            ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        // GET: AccountSubControl/Edit/5
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

            tblAccountSubControl tblAccountSubControl = _db.tblAccountSubControl.Find(id);
            if (tblAccountSubControl == null)
            {
                return HttpNotFound();
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        // POST: AccountSubControl/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountSubControl tblAccountSubControl)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int userID = Convert.ToInt32(Session["UserID"]);

            tblAccountSubControl.UserID = userID;
            tblAccountSubControl.AccountHeadID = _db.tblAccountControl.Find(tblAccountSubControl.AccountControlID)?.AccountHeadID ?? 0;

            if (ModelState.IsValid)
            {
                try
                {
                    var findSubControl = _db.tblAccountSubControl.FirstOrDefault(s => s.CompanyID == tblAccountSubControl.CompanyID
                        && s.BranchID == tblAccountSubControl.BranchID
                        && s.AccountSubControlName == tblAccountSubControl.AccountSubControlName
                        && s.AccountSubControlID != tblAccountSubControl.AccountSubControlID);

                    if (findSubControl == null)
                    {
                        _db.Entry(tblAccountSubControl).State = EntityState.Modified;
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Already Exist!";
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                    return RedirectToAction("EP500", "EP");
                }
            }

            ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"]));
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