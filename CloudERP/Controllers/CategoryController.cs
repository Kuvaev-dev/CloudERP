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

        private int GetCompanyID()
        {
            return Convert.ToInt32(Session["CompanyID"]);
        }

        private int GetBranchID()
        {
            return Convert.ToInt32(Session["BranchID"]);
        }

        private int GetUserID()
        {
            return Convert.ToInt32(Session["UserID"]);
        }

        private bool IsUserAuthenticated()
        {
            return !string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"]));
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
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = GetCompanyID();
            int branchID = GetBranchID();

            var tblCategory = _db.tblCategory.Include(t => t.tblBranch).Include(t => t.tblCompany)
                                             .Include(t => t.tblUser)
                                             .Where(c => c.CompanyID == companyID && c.BranchID == branchID);

            return View(tblCategory.ToList());
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            if (!IsUserAuthenticated())
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
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = GetCompanyID();
            int branchID = GetBranchID();
            int userID = GetUserID();

            tblCategory.BranchID = branchID;
            tblCategory.CompanyID = companyID;
            tblCategory.UserID = userID;

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
                    ViewBag.Message = "Category already exists.";
                }
            }

            return View(tblCategory);
        }

        // GET: Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblCategory tblCategory = _db.tblCategory.Find(id);

            if (tblCategory == null)
            {
                return HttpNotFound();
            }

            return View(tblCategory);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCategory tblCategory)
        {
            if (!IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Home");
            }

            int userID = GetUserID();
            tblCategory.UserID = userID;

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
                    ViewBag.Message = "Category already exists.";
                }
            }

            return View(tblCategory);
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
