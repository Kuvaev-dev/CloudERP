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
    public class BranchController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: Branch
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblBranch = db.tblBranch.Include(t => t.tblBranchType).Where(c => c.CompanyID == companyID);
            return View(tblBranch.ToList());
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
            tblBranch tblBranch = db.tblBranch.Find(id);
            if (tblBranch == null)
            {
                return HttpNotFound();
            }
            return View(tblBranch);
        }

        // GET: Branch/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            ViewBag.BrchID = new SelectList(db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName", 0);
            ViewBag.BranchTypeID = new SelectList(db.tblBranchType, "BranchTypeID", "BranchType", 0);
            return View();
        }

        // POST: Branch/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblBranch tblBranch)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            tblBranch.CompanyID = companyID;
            if (ModelState.IsValid)
            {
                db.tblBranch.Add(tblBranch);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrchID = new SelectList(db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName");
            ViewBag.BranchTypeID = new SelectList(db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);
            return View(tblBranch);
        }

        // GET: Branch/Edit/5
        public ActionResult Edit(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblBranch tblBranch = db.tblBranch.Find(id);
            if (tblBranch == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrchID = new SelectList(db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName", tblBranch.BrchID);
            ViewBag.BranchTypeID = new SelectList(db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);
            return View(tblBranch);
        }

        // POST: Branch/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblBranch tblBranch)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            tblBranch.CompanyID = companyID;
            if (ModelState.IsValid)
            {
                db.Entry(tblBranch).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrchID = new SelectList(db.tblBranch.Where(c => c.CompanyID == companyID).ToList(), "BranchID", "BranchName", tblBranch.BrchID);
            ViewBag.BranchTypeID = new SelectList(db.tblBranchType, "BranchTypeID", "BranchType", tblBranch.BranchTypeID);
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
            tblBranch tblBranch = db.tblBranch.Find(id);
            if (tblBranch == null)
            {
                return HttpNotFound();
            }
            return View(tblBranch);
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
            tblBranch tblBranch = db.tblBranch.Find(id);
            db.tblBranch.Remove(tblBranch);
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
