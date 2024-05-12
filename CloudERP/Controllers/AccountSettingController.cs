using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CloudERP.Models;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class AccountSettingController : Controller
    {
        private readonly CloudDBEntities _db;

        public AccountSettingController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: AccountSetting
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
            var tblAccountSetting = _db.tblAccountSetting.Include(t => t.tblAccountActivity).Include(t => t.tblAccountControl)
                                                        .Include(t => t.tblAccountHead).Include(t => t.tblBranch)
                                                        .Include(t => t.tblCompany).Where(t => t.CompanyID == companyID && t.BranchID == branchID);
            return View(tblAccountSetting.ToList());
        }

        // GET: AccountSetting/Create
        public ActionResult Create()
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
            ViewBag.AccountActivityID = new SelectList(_db.tblAccountActivity, "AccountActivityID", "Name", "0");
            ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "AccountControlID", "AccountControlName", "0");
            ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", "0");
            ViewBag.AccountSubControlID = new SelectList(_db.tblAccountSubControl.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "AccountSubControlID", "AccountSubControlName", "0");
            return View();
        }

        // POST: AccountSetting/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountSetting tblAccountSetting)
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
            tblAccountSetting.BranchID = branchID;
            tblAccountSetting.CompanyID = companyID;
            if (ModelState.IsValid)
            {
                var findSetting = _db.tblAccountSetting.Where(c => c.CompanyID == tblAccountSetting.CompanyID && c.BranchID == tblAccountSetting.BranchID && c.AccountActivityID == tblAccountSetting.AccountActivityID).FirstOrDefault();
                if (findSetting == null)
                {
                    _db.tblAccountSetting.Add(tblAccountSetting);
                    _db.SaveChanges();
                    ViewBag.Message = "Saved Successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            ViewBag.AccountActivityID = new SelectList(_db.tblAccountActivity, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
            ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
            ViewBag.AccountSubControlID = new SelectList(_db.tblAccountSubControl.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);
            return View(tblAccountSetting);
        }

        // GET: AccountSetting/Edit/5
        public ActionResult Edit(int? id)
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
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountSetting tblAccountSetting = _db.tblAccountSetting.Find(id);
            if (tblAccountSetting == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountActivityID = new SelectList(_db.tblAccountActivity, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
            ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
            ViewBag.AccountSubControlID = new SelectList(_db.tblAccountSubControl.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);
            return View(tblAccountSetting);
        }

        // POST: AccountSetting/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountSetting tblAccountSetting)
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
            if (ModelState.IsValid)
            {
                var findSetting = _db.tblAccountSetting.Where(c => c.CompanyID == tblAccountSetting.CompanyID && c.BranchID == tblAccountSetting.BranchID && c.AccountActivityID == tblAccountSetting.AccountActivityID && c.AccountSettingID != tblAccountSetting.AccountSettingID).FirstOrDefault();
                if (findSetting == null)
                {
                    _db.Entry(tblAccountSetting).State = EntityState.Modified;
                    _db.SaveChanges();
                    ViewBag.Message = "Updated Successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }
            ViewBag.AccountActivityID = new SelectList(_db.tblAccountActivity, "AccountActivityID", "Name", tblAccountSetting.AccountActivityID);
            ViewBag.AccountControlID = new SelectList(_db.tblAccountControl.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSetting.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSetting.AccountHeadID);
            ViewBag.AccountSubControlID = new SelectList(_db.tblAccountSubControl.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "AccountSubControlID", "AccountSubControlName", tblAccountSetting.AccountSubControlID);
            return View(tblAccountSetting);
        }

        [HttpGet]
        public ActionResult GetAccountControls(int? id)
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

            List<AccountControlMV> controls = new List<AccountControlMV>();
            var controlList = _db.tblAccountControl.Where(p => p.BranchID == branchID && p.CompanyID == companyID && p.AccountHeadID == id).ToList();
            foreach (var item in controlList)
            {
                controls.Add(new AccountControlMV() { AccountControlID = item.AccountControlID, AccountControlName = item.AccountControlName });
            }

            return Json(new { data = controls }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSubControls(int? id)
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

            List<AccountSubControlMV> subControls = new List<AccountSubControlMV>();
            var subControlList = _db.tblAccountSubControl.Where(p => p.BranchID == branchID && p.CompanyID == companyID && p.AccountControlID == id).ToList();
            foreach (var item in subControlList)
            {
                subControls.Add(new AccountSubControlMV() { AccountSubControlID = item.AccountSubControlID, AccountSubControlName = item.AccountSubControlName });
            }

            return Json(new { data = subControls }, JsonRequestBehavior.AllowGet);
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
