using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class AccountSubControlController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: AccountSubControl
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
            var tblAccountSubControl = db.tblAccountSubControl.Include(t => t.tblAccountControl).Include(t => t.tblAccountHead)
                                                              .Include(t => t.tblBranch).Include(t => t.tblUser)
                                                              .Where(t => t.CompanyID == companyID && t.BranchID == branchID);
            return View(tblAccountSubControl.ToList());
        }

        // GET: AccountSubControl/Create
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
            ViewBag.AccountControlID = new SelectList(db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID), "AccountControlID", "AccountControlName", "0");
            return View();
        }

        // POST: AccountSubControl/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountSubControl tblAccountSubControl)
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
            tblAccountSubControl.CompanyID = companyID;
            tblAccountSubControl.BranchID = branchID;
            tblAccountSubControl.UserID = userID;
            tblAccountSubControl.AccountHeadID = db.tblAccountControl.Find(tblAccountSubControl.AccountControlID).AccountHeadID;
            if (ModelState.IsValid)
            {
                var findSubControl = db.tblAccountSubControl.Where(s => s.CompanyID == tblAccountSubControl.CompanyID
                                                                  && s.BranchID == tblAccountSubControl.BranchID
                                                                  && s.AccountSubControlName == tblAccountSubControl.AccountSubControlName).FirstOrDefault();
                if (findSubControl == null)
                {
                    db.tblAccountSubControl.Add(tblAccountSubControl);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            ViewBag.AccountControlID = new SelectList(db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        // GET: AccountSubControl/Edit/5
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
            tblAccountSubControl tblAccountSubControl = db.tblAccountSubControl.Find(id);
            if (tblAccountSubControl == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountControlID = new SelectList(db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        // POST: AccountSubControl/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountSubControl tblAccountSubControl)
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
            tblAccountSubControl.UserID = userID;
            tblAccountSubControl.AccountHeadID = db.tblAccountControl.Find(tblAccountSubControl.AccountControlID).AccountHeadID;
            if (ModelState.IsValid)
            {
                var findSubControl = db.tblAccountSubControl.Where(s => s.CompanyID == tblAccountSubControl.CompanyID
                                                                  && s.BranchID == tblAccountSubControl.BranchID
                                                                  && s.AccountSubControlName == tblAccountSubControl.AccountSubControlName
                                                                  && s.AccountSubControlID != tblAccountSubControl.AccountSubControlID).FirstOrDefault();
                if (findSubControl == null)
                {
                    db.Entry(tblAccountSubControl).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            ViewBag.AccountControlID = new SelectList(db.tblAccountControl.Where(a => a.BranchID == branchID && a.CompanyID == companyID), "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            return View(tblAccountSubControl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
