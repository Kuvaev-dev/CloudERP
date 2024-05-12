using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class StockController : Controller
    {
        private readonly CloudDBEntities _db;

        public StockController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: Stock
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
            var tblStock = _db.tblStock.Include(t => t.tblBranch)
                .Include(t => t.tblCategory).Include(t => t.tblCompany).Include(t => t.tblUser)
                .Where(t => t.CompanyID == companyID && t.BranchID == branchID);
            return View(tblStock.ToList());
        }

        // GET: Stock/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStock tblStock = _db.tblStock.Find(id);
            if (tblStock == null)
            {
                return HttpNotFound();
            }
            return View(tblStock);
        }

        // GET: Stock/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            ViewBag.CategoryID = new SelectList(_db.tblCategory.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "CategoryID", "CategoryName", "0");
            return View();
        }

        // POST: Stock/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblStock tblStock)
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
            tblStock.BranchID = branchID;
            tblStock.CompanyID = companyID;
            tblStock.UserID = userID;
            if (ModelState.IsValid)
            {
                var findProduct = _db.tblStock.Where(p => p.CompanyID == companyID && p.BranchID == branchID && p.ProductName == tblStock.ProductName).FirstOrDefault();
                if (findProduct == null)
                {
                    _db.tblStock.Add(tblStock);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already In Stock";
                }
                
            }

            ViewBag.CategoryID = new SelectList(_db.tblCategory.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "CategoryID", "CategoryName", tblStock.CategoryID);
            return View(tblStock);
        }

        // GET: Stock/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStock tblStock = _db.tblStock.Find(id);
            if (tblStock == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryID = new SelectList(_db.tblCategory.Where(c => c.BranchID == tblStock.BranchID && c.CompanyID == tblStock.CompanyID), "CategoryID", "CategoryName", tblStock.CategoryID);
            return View(tblStock);
        }

        // POST: Stock/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblStock tblStock)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblStock.UserID = userID;
            if (ModelState.IsValid)
            {
                var findProduct = _db.tblStock.Where(p => p.CompanyID == tblStock.CompanyID && p.BranchID == tblStock.BranchID && p.ProductName == tblStock.ProductName && p.ProductID != tblStock.ProductID).FirstOrDefault();
                if (findProduct == null)
                {
                    _db.Entry(tblStock).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already In Stock";
                }
            }

            ViewBag.CategoryID = new SelectList(_db.tblCategory.Where(c => c.BranchID == tblStock.BranchID && c.CompanyID == tblStock.CompanyID), "CategoryID", "CategoryName", tblStock.CategoryID);
            return View(tblStock);
        }

        // GET: Stock/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblStock tblStock = _db.tblStock.Find(id);
            if (tblStock == null)
            {
                return HttpNotFound();
            }
            return View(tblStock);
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblStock tblStock = _db.tblStock.Find(id);
            _db.tblStock.Remove(tblStock);
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
