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
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            try
            {
                var tblAccountSubControl = _db.tblAccountSubControl
                    .Include(t => t.tblAccountControl)
                    .Include(t => t.tblAccountHead)
                    .Include(t => t.tblBranch)
                    .Include(t => t.tblUser)
                    .Where(t => t.CompanyID == companyID && t.BranchID == branchID);

                return View(tblAccountSubControl.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while fetching data: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: AccountSubControl/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            try
            {
                ViewBag.AccountControlID = new SelectList(
                    _db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID),
                    "AccountControlID",
                    "AccountControlName",
                    "0"
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while fetching data: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return View(new tblAccountSubControl());
        }

        // POST: AccountSubControl/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountSubControl tblAccountSubControl)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
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
                    TempData["ErrorMessage"] = "An unexpected error occurred while creating the record: " + ex.Message;
                    return RedirectToAction("EP500", "EP");
                }
            }

            try
            {
                ViewBag.AccountControlID = new SelectList(
                    _db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID),
                    "AccountControlID",
                    "AccountControlName",
                    tblAccountSubControl.AccountControlID
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while fetching data: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return View(tblAccountSubControl);
        }

        // GET: AccountSubControl/Edit/5
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

            tblAccountSubControl tblAccountSubControl;

            try
            {
                tblAccountSubControl = _db.tblAccountSubControl.Find(id);
                if (tblAccountSubControl == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while fetching data: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            try
            {
                ViewBag.AccountControlID = new SelectList(
                    _db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID),
                    "AccountControlID",
                    "AccountControlName",
                    tblAccountSubControl.AccountControlID
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while fetching data: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return View(tblAccountSubControl);
        }

        // POST: AccountSubControl/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountSubControl tblAccountSubControl)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
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
                    TempData["ErrorMessage"] = "An unexpected error occurred while updating the record: " + ex.Message;
                    return RedirectToAction("EP500", "EP");
                }
            }

            try
            {
                ViewBag.AccountControlID = new SelectList(
                    _db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID),
                    "AccountControlID",
                    "AccountControlName",
                    tblAccountSubControl.AccountControlID
                );
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while fetching data: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return View(tblAccountSubControl);
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