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
    public class AccountControlController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly List<AccountControlMV> accountControl = new List<AccountControlMV>();

        public AccountControlController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: AccountControl
        public ActionResult Index()
        {
            accountControl.Clear();
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
            var tblAccountControl = _db.tblAccountControl.Include(t => t.tblBranch).Include(t => t.tblCompany).Include(t => t.tblUser).Where(a => a.CompanyID == companyID && a.BranchID == branchID);
            foreach (var item in tblAccountControl)
            {
                accountControl.Add(new AccountControlMV
                {
                    AccountControlID = item.AccountControlID,
                    AccountControlName = item.AccountControlName,
                    AccountHeadID = item.AccountHeadID,
                    AccountHeadName = _db.tblAccountHead.Find(item.AccountHeadID).AccountHeadName,
                    BranchID = item.BranchID,
                    BranchName = item.tblBranch.BranchName,
                    CompanyID = item.CompanyID,
                    Name = item.tblCompany.Name,
                    UserID = item.UserID,
                    UserName = item.tblUser.UserName
                });
            }
            return View(accountControl.ToList());
        }

        // GET: AccountControl/Create
        public ActionResult Create()
        {
            ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName");
            return View(new tblAccountControl());
        }

        // POST: AccountControl/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountControl tblAccountControl)
        {
            accountControl.Clear();
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
            tblAccountControl.CompanyID = companyID;
            tblAccountControl.BranchID = branchID;
            tblAccountControl.UserID = userID;
            if (ModelState.IsValid)
            {
                var findControl = _db.tblAccountControl.Where(a => a.CompanyID == companyID && a.BranchID == branchID && a.AccountControlName == tblAccountControl.AccountControlName).FirstOrDefault();
                if (findControl == null)
                {
                    _db.tblAccountControl.Add(tblAccountControl);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
            return View(tblAccountControl);
        }

        // GET: AccountControl/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountControl tblAccountControl = _db.tblAccountControl.Find(id);
            if (tblAccountControl == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
            return View(tblAccountControl);
        }

        // POST: AccountControl/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountControl tblAccountControl)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblAccountControl.UserID = userID;
            if (ModelState.IsValid)
            {
                var findControl = _db.tblAccountControl.Where(a => a.CompanyID == tblAccountControl.CompanyID
                                                               && a.BranchID == tblAccountControl.BranchID
                                                               && a.AccountControlName == tblAccountControl.AccountControlName
                                                               && a.AccountControlID != tblAccountControl.AccountControlID).FirstOrDefault();
                if (findControl == null)
                {
                    _db.Entry(tblAccountControl).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            ViewBag.AccountHeadID = new SelectList(_db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountControl.AccountHeadID);
            return View(tblAccountControl);
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
