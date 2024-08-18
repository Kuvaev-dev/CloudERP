using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class CategoryController : Controller
    {
        private readonly CloudDBEntities _db;

        public CategoryController(CloudDBEntities db)
        {
            _db = db;
        }

        private bool CategoryExists(int companyID, int branchID, string categoryName, int categoryId)
        {
            return _db.tblCategory.Any(c => c.CompanyID == companyID
                                        && c.BranchID == branchID
                                        && c.CategoryName == categoryName
                                        && c.CategoryID != categoryId);
        }

        // GET: Category
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            try
            {
                var tblCategory = _db.tblCategory.Include(t => t.tblBranch).Include(t => t.tblCompany)
                                                 .Include(t => t.tblUser)
                                                 .Where(c => c.CompanyID == companyID && c.BranchID == branchID);

                return View(tblCategory.ToList());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View(new tblCategory());
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblCategory tblCategory)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int userID = Convert.ToInt32(Session["UserID"]);

            tblCategory.BranchID = branchID;
            tblCategory.CompanyID = companyID;
            tblCategory.UserID = userID;

            try
            {
                if (ModelState.IsValid)
                {
                    if (!CategoryExists(companyID, branchID, tblCategory.CategoryName, tblCategory.CategoryID))
                    {
                        _db.tblCategory.Add(tblCategory);
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = Resources.Messages.AlreadyExists;
                    }
                }

                return View(tblCategory);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Category/Edit/5
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
                tblCategory tblCategory = _db.tblCategory.Find(id);

                if (tblCategory == null)
                {
                    return HttpNotFound();
                }

                return View(tblCategory);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCategory tblCategory)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userID = Convert.ToInt32(Session["UserID"]);
            tblCategory.UserID = userID;

            try
            {
                if (ModelState.IsValid)
                {
                    int companyID = tblCategory.CompanyID;
                    int branchID = tblCategory.BranchID;

                    if (!CategoryExists(companyID, branchID, tblCategory.CategoryName, tblCategory.CategoryID))
                    {
                        _db.Entry(tblCategory).State = EntityState.Modified;
                        _db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Message = Resources.Messages.AlreadyExists;
                    }
                }

                return View(tblCategory);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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