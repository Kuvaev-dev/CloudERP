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

            try
            {
                return View(_db.tblCompany.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving companies: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                tblCompany tblCompany = _db.tblCompany.Find(id);

                if (tblCompany == null)
                {
                    return HttpNotFound();
                }

                return View(tblCompany);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving company details: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                if (ModelState.IsValid)
                {
                    if (tblCompany.LogoFile != null)
                    {
                        var folder = "~/Content/CompanyLogo";
                        var file = $"{tblCompany.CompanyID}.jpg";
                        var response = FileHelper.UploadPhoto(tblCompany.LogoFile, folder, file);

                        if (response)
                        {
                            var filePath = Server.MapPath($"{folder}/{file}");
                            if (System.IO.File.Exists(filePath))
                            {
                                tblCompany.Logo = $"{folder}/{file}";
                            }
                            else
                            {
                                tblCompany.Logo = "~/Content/CompanyLogo/erp-logo.png";
                            }
                        }
                        else
                        {
                            tblCompany.Logo = "~/Content/CompanyLogo/erp-logo.png";
                        }
                    }
                    else
                    {
                        tblCompany.Logo = "~/Content/CompanyLogo/erp-logo.png";
                    }

                    _db.tblCompany.Add(tblCompany);
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while creating the company: " + ex.Message;
                return RedirectToAction("EP500", "EP");
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

            try
            {
                tblCompany tblCompany = _db.tblCompany.Find(id);

                if (tblCompany == null)
                {
                    return HttpNotFound();
                }

                return View(tblCompany);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving the company for editing: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                if (ModelState.IsValid)
                {
                    if (tblCompany.LogoFile != null)
                    {
                        var folder = "~/Content/CompanyLogo";
                        var file = $"{tblCompany.CompanyID}.jpg";
                        var response = FileHelper.UploadPhoto(tblCompany.LogoFile, folder, file);

                        if (response)
                        {
                            tblCompany.Logo = $"{folder}/{file}";
                        }
                    }
                    else
                    {
                        var existingCompany = _db.tblCompany.AsNoTracking().FirstOrDefault(c => c.CompanyID == tblCompany.CompanyID);
                        if (existingCompany != null)
                        {
                            tblCompany.Logo = existingCompany.Logo;
                        }
                    }

                    _db.Entry(tblCompany).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while updating the company: " + ex.Message;
                return RedirectToAction("EP500", "EP");
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

            try
            {
                tblCompany tblCompany = _db.tblCompany.Find(id);

                if (tblCompany == null)
                {
                    return HttpNotFound();
                }

                return View(tblCompany);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving the company for deletion: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

            try
            {
                tblCompany tblCompany = _db.tblCompany.Find(id);
                if (tblCompany != null)
                {
                    _db.tblCompany.Remove(tblCompany);
                    _db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the company: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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