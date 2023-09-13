using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CloudERP.Helpers;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class CompanyController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        // GET: Company
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View(db.tblCompany.ToList());
        }

        // GET: Company/Details/5
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
            tblCompany tblCompany = db.tblCompany.Find(id);
            if (tblCompany == null)
            {
                return HttpNotFound();
            }
            return View(tblCompany);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View();
        }

        // POST: Company/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblCompany tblCompany)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                db.tblCompany.Add(tblCompany);
                db.SaveChanges();

                if (tblCompany.LogoFile != null)
                {
                    var folder = "~/Content/CompanyLogo";
                    var file = string.Format("{0}.jpg", tblCompany.CompanyID);
                    var response = FileHelper.UploadPhoto(tblCompany.LogoFile, folder, file);
                    if (response)
                    {
                        var picture = string.Format("{0}/{1}", folder, file);
                        tblCompany.Logo = picture;
                        db.Entry(tblCompany).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }

            return View(tblCompany);
        }

        // GET: Company/Edit/5
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
            tblCompany tblCompany = db.tblCompany.Find(id);
            if (tblCompany == null)
            {
                return HttpNotFound();
            }
            return View(tblCompany);
        }

        // POST: Company/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCompany tblCompany)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                if (tblCompany.LogoFile != null)
                {
                    var folder = "~/Content/CompanyLogo";
                    var file = string.Format("{0}.jpg", tblCompany.CompanyID);
                    var response = FileHelper.UploadPhoto(tblCompany.LogoFile, folder, file);
                    if (response)
                    {
                        var picture = string.Format("{0}/{1}", folder, file);
                        tblCompany.Logo = picture;
                    }
                }

                db.Entry(tblCompany).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblCompany);
        }

        // GET: Company/Delete/5
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
            tblCompany tblCompany = db.tblCompany.Find(id);
            if (tblCompany == null)
            {
                return HttpNotFound();
            }
            return View(tblCompany);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblCompany tblCompany = db.tblCompany.Find(id);
            db.tblCompany.Remove(tblCompany);
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
