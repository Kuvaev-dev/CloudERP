using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class FinancialYearController : Controller
    {
        private readonly CloudDBEntities db = new CloudDBEntities();

        // GET: FinancialYear
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var tblFinancialYear = db.tblFinancialYear.Include(t => t.tblUser);
            return View(tblFinancialYear.ToList());
        }

        // GET: FinancialYear/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        // POST: FinancialYear/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblFinancialYear tblFinancialYear)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblFinancialYear.UserID = userID;
            if (ModelState.IsValid)
            {
                var findFinancialYear = db.tblFinancialYear.Where(f => f.FinancialYear == tblFinancialYear.FinancialYear).FirstOrDefault();
                if (findFinancialYear == null)
                {
                    db.tblFinancialYear.Add(tblFinancialYear);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Aready Exist!";
                }
            }

            return View(tblFinancialYear);
        }

        // GET: FinancialYear/Edit/5
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
            tblFinancialYear tblFinancialYear = db.tblFinancialYear.Find(id);
            if (tblFinancialYear == null)
            {
                return HttpNotFound();
            }
 
            return View(tblFinancialYear);
        }

        // POST: FinancialYear/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblFinancialYear tblFinancialYear)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblFinancialYear.UserID = userID;
            if (ModelState.IsValid)
            {
                var findFinancialYear = db.tblFinancialYear.Where(f => f.FinancialYear == tblFinancialYear.FinancialYear
                                                                    && f.FinancialYearID != tblFinancialYear.FinancialYearID).FirstOrDefault();
                if (findFinancialYear == null)
                {
                    db.Entry(tblFinancialYear).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Aready Exist!";
                }
            }

            return View(tblFinancialYear);
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
