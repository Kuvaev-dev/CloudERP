using System;
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

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int userID = Convert.ToInt32(Session["UserID"]);

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
            return View(new tblAccountHead());
        }

        // POST: AccountHead/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblAccountHead tblAccountHead)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userID = Convert.ToInt32(Session["UserID"]);
            tblAccountHead.UserID = userID;

            if (ModelState.IsValid)
            {
                try
                {
                    var findHead = _db.tblAccountHead.FirstOrDefault(a => a.AccountHeadName == tblAccountHead.AccountHeadName);
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
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                    return RedirectToAction("EP500", "EP");
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblAccountHead tblAccountHead)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int userID = Convert.ToInt32(Session["UserID"]);
            tblAccountHead.UserID = userID;

            if (ModelState.IsValid)
            {
                try
                {
                    var findHead = _db.tblAccountHead.FirstOrDefault(a => a.AccountHeadName == tblAccountHead.AccountHeadName
                                                                        && a.AccountHeadID != tblAccountHead.AccountHeadID);
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
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                    return RedirectToAction("EP500", "EP");
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
            try
            {
                tblAccountHead tblAccountHead = _db.tblAccountHead.Find(id);
                if (tblAccountHead == null)
                {
                    return HttpNotFound();
                }

                _db.tblAccountHead.Remove(tblAccountHead);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

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