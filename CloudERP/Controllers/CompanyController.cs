using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CloudERP.Helpers;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class CompanyController : Controller
    {
        private readonly CloudDBEntities _db;

        public CompanyController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: Company
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View(_db.tblCompany.ToList());
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

            tblCompany tblCompany = _db.tblCompany.Find(id);

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

            return View(new tblCompany());
        }

        // POST: Company/Create
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
                _db.tblCompany.Add(tblCompany);
                _db.SaveChanges();

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

                _db.Entry(tblCompany).State = EntityState.Modified;
                _db.SaveChanges();

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

            tblCompany tblCompany = _db.tblCompany.Find(id);

            if (tblCompany == null)
            {
                return HttpNotFound();
            }

            return View(tblCompany);
        }

        // POST: Company/Edit/5
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

                _db.Entry(tblCompany).State = EntityState.Modified;
                _db.SaveChanges();

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

            tblCompany tblCompany = _db.tblCompany.Find(id);

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

            tblCompany tblCompany = _db.tblCompany.Find(id);
            _db.tblCompany.Remove(tblCompany);
            _db.SaveChanges();

            return RedirectToAction("Index");
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