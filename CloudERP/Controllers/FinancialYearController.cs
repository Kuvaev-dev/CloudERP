using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class FinancialYearController : Controller
    {
        private readonly CloudDBEntities _db;

        public FinancialYearController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: FinancialYear
        public ActionResult Index()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var tblFinancialYear = _db.tblFinancialYear.Include(t => t.tblUser);

                return View(tblFinancialYear.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: FinancialYear/Create
        public ActionResult Create()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(new tblFinancialYear());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: FinancialYear/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblFinancialYear tblFinancialYear)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int userID = Convert.ToInt32(Session["UserID"]);
                tblFinancialYear.UserID = userID;

                if (ModelState.IsValid)
                {
                    var findFinancialYear = _db.tblFinancialYear.FirstOrDefault(f => f.FinancialYear == tblFinancialYear.FinancialYear);
                    if (findFinancialYear == null)
                    {
                        _db.tblFinancialYear.Add(tblFinancialYear);
                        _db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Financial year already exists!";
                    }
                }

                return View(tblFinancialYear);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: FinancialYear/Edit/5
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

                tblFinancialYear tblFinancialYear = _db.tblFinancialYear.Find(id);
                if (tblFinancialYear == null)
                {
                    return HttpNotFound();
                }

                return View(tblFinancialYear);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: FinancialYear/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblFinancialYear tblFinancialYear)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int userID = Convert.ToInt32(Session["UserID"]);
                tblFinancialYear.UserID = userID;

                if (ModelState.IsValid)
                {
                    var findFinancialYear = _db.tblFinancialYear.FirstOrDefault(f => f.FinancialYear == tblFinancialYear.FinancialYear
                                                                        && f.FinancialYearID != tblFinancialYear.FinancialYearID);
                    if (findFinancialYear == null)
                    {
                        _db.Entry(tblFinancialYear).State = EntityState.Modified;
                        _db.SaveChanges();

                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = "Financial year already exists!";
                    }
                }

                return View(tblFinancialYear);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
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