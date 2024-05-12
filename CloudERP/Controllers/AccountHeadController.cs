using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class AccountHeadController : Controller
    {
        private readonly CloudDBEntities _db;

        public AccountHeadController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: AccountHead
        public ActionResult Index()
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
            var tblAccountHead = _db.tblAccountHead.Include(t => t.tblUser).ToList();
            return View(tblAccountHead.ToList());
        }

        // GET: AccountHead/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountHead tblAccountHead = _db.tblAccountHead.Find(id);
            if (tblAccountHead == null)
            {
                return HttpNotFound();
            }
            return View(tblAccountHead);
        }

        // GET: AccountHead/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccountHead/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountHead tblAccountHead)
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
            tblAccountHead.UserID = userID;
            if (ModelState.IsValid)
            {
                var findHead = _db.tblAccountHead.Where(a => a.AccountHeadName == tblAccountHead.AccountHeadName).FirstOrDefault();
                if (findHead == null)
                {
                    _db.tblAccountHead.Add(tblAccountHead);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            return View(tblAccountHead);
        }

        // GET: AccountHead/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountHead tblAccountHead = _db.tblAccountHead.Find(id);
            if (tblAccountHead == null)
            {
                return HttpNotFound();
            }

            return View(tblAccountHead);
        }

        // POST: AccountHead/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountHead tblAccountHead)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int userID = 0;
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            tblAccountHead.UserID = userID;
            if (ModelState.IsValid)
            {
                var findHead = _db.tblAccountHead.Where(a => a.AccountHeadName == tblAccountHead.AccountHeadName
                                                         && a.AccountHeadID != tblAccountHead.AccountHeadID).FirstOrDefault();
                if (findHead == null)
                {
                    _db.Entry(tblAccountHead).State = EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = "Already Exist!";
                }
            }

            return View(tblAccountHead);
        }

        // GET: AccountHead/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblAccountHead tblAccountHead = _db.tblAccountHead.Find(id);
            if (tblAccountHead == null)
            {
                return HttpNotFound();
            }
            return View(tblAccountHead);
        }

        // POST: AccountHead/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblAccountHead tblAccountHead = _db.tblAccountHead.Find(id);
            _db.tblAccountHead.Remove(tblAccountHead);
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
