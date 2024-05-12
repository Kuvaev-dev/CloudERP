using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DatabaseAccess;

namespace CloudERP.Controllers
{
    public class UserController : Controller
    {
        private readonly CloudDBEntities _db;

        public UserController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: User
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var tblUser = _db.tblUser.Include(t => t.tblUserType);
            return View(tblUser.ToList());
        }

        public ActionResult SubBranchUser()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyID);
            int branchTypeID = 0;
            int.TryParse(Convert.ToString(Session["BranchTypeID"]), out branchTypeID);
            int brchID = 0;
            brchID = Convert.ToInt32(Convert.ToString(Session["BrchID"]));
            if (branchTypeID == 1)  // Main Branch
            {
                var tblUser = from s in _db.tblUser
                              join sa in _db.tblEmployee on s.UserID equals sa.UserID
                              where sa.CompanyID == companyID
                              select s;

                foreach (var item in tblUser)
                {
                    item.FullName = item.FullName + "(" + _db.tblEmployee.Where(e => e.UserID == item.UserID).FirstOrDefault().tblBranch.BranchName + ")";
                }
                return View(tblUser.ToList());
            }
            else
            {
                var tblUser = from s in _db.tblUser
                              join sa in _db.tblEmployee on s.UserID equals sa.UserID
                              where sa.tblBranch.BrchID == brchID
                              select s;

                foreach (var item in tblUser)
                {
                    item.FullName = item.FullName + "(" + _db.tblEmployee.Where(e => e.UserID == item.UserID).FirstOrDefault().tblBranch.BranchName + ")";
                }

                return View(tblUser.ToList());
            }
        }

        // GET: User/Details/5
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
            tblUser tblUser = _db.tblUser.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            return View(tblUser);
        }

        // GET: User/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.UserTypeID = new SelectList(_db.tblUserType, "UserTypeID", "UserType");
            return View();
        }

        // POST: User/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblUser tblUser)
        {
            int companyID = 0;
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyID);
            if (companyID == 0)
            {
                tblUser.UserTypeID = 1;
            }
            else
            {
                tblUser.UserTypeID = 2;
            }
            
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                _db.tblUser.Add(tblUser);
                _db.SaveChanges();
                if (companyID == 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("SubBranchUser");
                }
            }

            ViewBag.UserTypeID = new SelectList(_db.tblUserType, "UserTypeID", "UserType", tblUser.UserTypeID);
            return View(tblUser);
        }

        // GET: User/Edit/5
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
            tblUser tblUser = _db.tblUser.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserTypeID = new SelectList(_db.tblUserType, "UserTypeID", "UserType", tblUser.UserTypeID);
            return View(tblUser);
        }

        // POST: User/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblUser tblUser)
        {
            int companyID = 0;
            int.TryParse(Convert.ToString(Session["CompanyID"]), out companyID);
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (ModelState.IsValid)
            {
                _db.Entry(tblUser).State = EntityState.Modified;
                _db.SaveChanges();
                if (companyID == 0)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("SubBranchUser");
                }
            }
            ViewBag.UserTypeID = new SelectList(_db.tblUserType, "UserTypeID", "UserType", tblUser.UserTypeID);
            return View(tblUser);
        }

        // GET: User/Delete/5
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
            tblUser tblUser = _db.tblUser.Find(id);
            if (tblUser == null)
            {
                return HttpNotFound();
            }
            return View(tblUser);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            tblUser tblUser = _db.tblUser.Find(id);
            _db.tblUser.Remove(tblUser);
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
