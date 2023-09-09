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
    public class UserTypeController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: UserType
        public ActionResult Index()
        {
            return View(db.tblUserType.ToList());
        }

        // GET: UserType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUserType tblUserType = db.tblUserType.Find(id);
            if (tblUserType == null)
            {
                return HttpNotFound();
            }
            return View(tblUserType);
        }

        // GET: UserType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserType/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserTypeID,UserType")] tblUserType tblUserType)
        {
            if (ModelState.IsValid)
            {
                db.tblUserType.Add(tblUserType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tblUserType);
        }

        // GET: UserType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUserType tblUserType = db.tblUserType.Find(id);
            if (tblUserType == null)
            {
                return HttpNotFound();
            }
            return View(tblUserType);
        }

        // POST: UserType/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserTypeID,UserType")] tblUserType tblUserType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblUserType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblUserType);
        }

        // GET: UserType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblUserType tblUserType = db.tblUserType.Find(id);
            if (tblUserType == null)
            {
                return HttpNotFound();
            }
            return View(tblUserType);
        }

        // POST: UserType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblUserType tblUserType = db.tblUserType.Find(id);
            db.tblUserType.Remove(tblUserType);
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
