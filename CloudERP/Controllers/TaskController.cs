using DatabaseAccess;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TaskController : Controller
    {
        private readonly CloudDBEntities _db;

        public TaskController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: Task
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int userID = Convert.ToInt32(Session["UserID"]);

            var tasks = _db.tblTask.Where(t => t.CompanyID == companyID && t.BranchID == branchID && t.UserID == userID).ToList();

            return View(tasks);
        }

        // GET: Task/Details/5
        public ActionResult Details(int id)
        {
            var task = _db.tblTask.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }

            return View(task);
        }

        // GET: Task/Create
        public ActionResult Create()
        {
            return View(new tblTask());
        }

        // POST: Task/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblTask task)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int userID = Convert.ToInt32(Session["UserID"]);

            task.BranchID = branchID;
            task.CompanyID = companyID;
            task.UserID = userID;

            if (ModelState.IsValid)
            {
                _db.tblTask.Add(task);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // GET: Task/Edit/5
        public ActionResult Edit(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var task = _db.tblTask.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }

            return View(task);
        }

        // POST: Task/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblTask task)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int userID = Convert.ToInt32(Session["UserID"]);

            task.BranchID = branchID;
            task.CompanyID = companyID;
            task.UserID = userID;

            if (ModelState.IsValid)
            {
                _db.Entry(task).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(task);
        }

        // GET: Task/Delete/5
        public ActionResult Delete(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var task = _db.tblTask.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }

            return View(task);
        }

        // POST: Task/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var task = _db.tblTask.Find(id);
            if (task != null)
            {
                _db.tblTask.Remove(task);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: Task/Complete/5
        public ActionResult Complete(int id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var task = _db.tblTask.Find(id);
            if (task == null)
            {
                return HttpNotFound();
            }
            task.IsCompleted = true;

            _db.Entry(task).State = System.Data.Entity.EntityState.Modified;
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