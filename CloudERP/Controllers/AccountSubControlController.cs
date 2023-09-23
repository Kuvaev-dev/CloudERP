using System;
using System.Collections.Generic;
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
            var tblAccountSubControl = db.tblAccountSubControl.Include(t => t.tblAccountControl).Include(t => t.tblAccountHead).Include(t => t.tblBranch).Include(t => t.tblUser);
            return View(tblAccountSubControl.ToList());
        }

        // GET: AccountSubControl/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountSubControl tblAccountSubControl = db.tblAccountSubControl.Find(id);
            if (tblAccountSubControl == null)
            {
                return HttpNotFound();
            }
            return View(tblAccountSubControl);
        }

        // GET: AccountSubControl/Create
        public ActionResult Create()
        {
            ViewBag.AccountControlID = new SelectList(db.tblAccountControl, "AccountControlID", "AccountControlName");
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHead, "AccountHeadID", "AccountHeadName");
            ViewBag.BranchID = new SelectList(db.tblBranch, "BranchID", "BranchName");
            ViewBag.UserID = new SelectList(db.tblUser, "UserID", "FullName");
            return View();
        }

        // POST: AccountSubControl/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AccountSubControlID,AccountHeadID,AccountControlID,CompanyID,BranchID,AccountSubControlName,UserID")] tblAccountSubControl tblAccountSubControl)
        {
            if (ModelState.IsValid)
            {
                db.tblAccountSubControl.Add(tblAccountSubControl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountControlID = new SelectList(db.tblAccountControl, "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSubControl.AccountHeadID);
            ViewBag.BranchID = new SelectList(db.tblBranch, "BranchID", "BranchName", tblAccountSubControl.BranchID);
            ViewBag.UserID = new SelectList(db.tblUser, "UserID", "FullName", tblAccountSubControl.UserID);
            return View(tblAccountSubControl);
        }

        // GET: AccountSubControl/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountSubControl tblAccountSubControl = db.tblAccountSubControl.Find(id);
            if (tblAccountSubControl == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountControlID = new SelectList(db.tblAccountControl, "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSubControl.AccountHeadID);
            ViewBag.BranchID = new SelectList(db.tblBranch, "BranchID", "BranchName", tblAccountSubControl.BranchID);
            ViewBag.UserID = new SelectList(db.tblUser, "UserID", "FullName", tblAccountSubControl.UserID);
            return View(tblAccountSubControl);
        }

        // POST: AccountSubControl/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AccountSubControlID,AccountHeadID,AccountControlID,CompanyID,BranchID,AccountSubControlName,UserID")] tblAccountSubControl tblAccountSubControl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblAccountSubControl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountControlID = new SelectList(db.tblAccountControl, "AccountControlID", "AccountControlName", tblAccountSubControl.AccountControlID);
            ViewBag.AccountHeadID = new SelectList(db.tblAccountHead, "AccountHeadID", "AccountHeadName", tblAccountSubControl.AccountHeadID);
            ViewBag.BranchID = new SelectList(db.tblBranch, "BranchID", "BranchName", tblAccountSubControl.BranchID);
            ViewBag.UserID = new SelectList(db.tblUser, "UserID", "FullName", tblAccountSubControl.UserID);
            return View(tblAccountSubControl);
        }

        // GET: AccountSubControl/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountSubControl tblAccountSubControl = db.tblAccountSubControl.Find(id);
            if (tblAccountSubControl == null)
            {
                return HttpNotFound();
            }
            return View(tblAccountSubControl);
        }

        // POST: AccountSubControl/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblAccountSubControl tblAccountSubControl = db.tblAccountSubControl.Find(id);
            db.tblAccountSubControl.Remove(tblAccountSubControl);
            db.SaveChanges();
            return RedirectToAction("Index");
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
