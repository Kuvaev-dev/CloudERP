using System;
using System.Data;
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

        // GET: Category
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            var tblCategory = _db.tblCategory.Include(t => t.tblBranch).Include(t => t.tblCompany)
                                            .Include(t => t.tblUser)
                                            .Where(c => c.CompanyID == companyID && c.BranchID == branchID);
            return View(tblCategory.ToList());
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
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblCategory tblCategory)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblCategory.BranchID = branchID;
            tblCategory.CompanyID = companyID;
            tblCategory.UserID = userID;
            if (ModelState.IsValid)
            {
                var findCategory = _db.tblCategory.Where(c => c.CompanyID == companyID 
                                                        && c.BranchID == branchID 
                                                        && c.CategoryName == tblCategory.CategoryName).FirstOrDefault();
                if (findCategory == null)
                {
                    _db.tblCategory.Add(tblCategory);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist";
                }
            }

            return View(tblCategory);
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
            tblCategory tblCategory = _db.tblCategory.Find(id);
            if (tblCategory == null)
            {
                return HttpNotFound();
            }

            return View(tblCategory);
        }

        // POST: Category/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblCategory tblCategory)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblCategory.UserID = userID;
            if (ModelState.IsValid)
            {
                var findCategory = _db.tblCategory.Where(c => c.CompanyID == tblCategory.CompanyID
                                                        && c.BranchID == tblCategory.BranchID
                                                        && c.CategoryName == tblCategory.CategoryName
                                                        && c.CategoryID != tblCategory.CategoryID).FirstOrDefault();
                if (findCategory == null)
                {
                    _db.Entry(tblCategory).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist";
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
