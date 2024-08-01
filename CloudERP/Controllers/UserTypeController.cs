using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class UserTypeController : Controller
    {
        private readonly CloudDBEntities _db;

        public UserTypeController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: UserType
        public ActionResult Index()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(_db.tblUserType.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving user types: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: UserType/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                tblUserType tblUserType = _db.tblUserType.Find(id);
                if (tblUserType == null)
                {
                    return HttpNotFound();
                }

                return View(tblUserType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving user type details: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: UserType/Create
        public ActionResult Create()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(new tblUserType());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while preparing to create a user type: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: UserType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblUserType tblUserType)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (ModelState.IsValid)
                {
                    _db.tblUserType.Add(tblUserType);
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(tblUserType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while creating the user type: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: UserType/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                tblUserType tblUserType = _db.tblUserType.Find(id);
                if (tblUserType == null)
                {
                    return HttpNotFound();
                }

                return View(tblUserType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving user type for editing: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: UserType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblUserType tblUserType)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (ModelState.IsValid)
                {
                    _db.Entry(tblUserType).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }

                return View(tblUserType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while updating the user type: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: UserType/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                tblUserType tblUserType = _db.tblUserType.Find(id);
                if (tblUserType == null)
                {
                    return HttpNotFound();
                }

                return View(tblUserType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while retrieving user type for deletion: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: UserType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                tblUserType tblUserType = _db.tblUserType.Find(id);
                _db.tblUserType.Remove(tblUserType);
                _db.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the user type: " + ex.Message;
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