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
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        // GET: UserType
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View(_db.tblUserType.ToList());
        }

        // GET: UserType/Details/5
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
            tblUserType tblUserType = _db.tblUserType.Find(id);
            if (tblUserType == null)
            {
                return HttpNotFound();
            }
            return View(tblUserType);
        }

        // GET: UserType/Create
        public ActionResult Create()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            return View(new tblUserType());
        }

        // POST: UserType/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblUserType tblUserType)
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

        // GET: UserType/Edit/5
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
            tblUserType tblUserType = _db.tblUserType.Find(id);
            if (tblUserType == null)
            {
                return HttpNotFound();
            }
            return View(tblUserType);
        }

        // POST: UserType/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblUserType tblUserType)
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

        // GET: UserType/Delete/5
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
            tblUserType tblUserType = _db.tblUserType.Find(id);
            if (tblUserType == null)
            {
                return HttpNotFound();
            }
            return View(tblUserType);
        }

        // POST: UserType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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
