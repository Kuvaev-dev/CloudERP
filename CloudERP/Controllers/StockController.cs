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
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var tblStock = _db.tblStock.Include(t => t.tblBranch)
                    .Include(t => t.tblCategory).Include(t => t.tblCompany).Include(t => t.tblUser)
                    .Where(t => t.CompanyID == companyID && t.BranchID == branchID)
                    .ToList();

                return View(tblStock);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/Details/5
        public ActionResult Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/Create
        public ActionResult Create()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                ViewBag.CategoryID = new SelectList(_db.tblCategory.Where(c => c.BranchID == branchID && c.CompanyID == companyID), "CategoryID", "CategoryName", "0");

                return View(new tblStock());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Stock/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblStock tblStock)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/Edit/5
        public ActionResult Edit(int? id)
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Stock/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblStock tblStock)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int userID = Convert.ToInt32(Session["UserID"]);
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Stock/Delete/5
        public ActionResult Delete(int? id)
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Stock/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                tblStock tblStock = _db.tblStock.Find(id);
                _db.tblStock.Remove(tblStock);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
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