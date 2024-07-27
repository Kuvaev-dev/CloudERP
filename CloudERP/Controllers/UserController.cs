using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using CloudERP.Helpers;
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
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var tblUser = _db.tblUser.Include(t => t.tblUserType);

                return View(tblUser.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult SubBranchUser()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchTypeID = Convert.ToInt32(Session["BranchTypeID"]);
                int brchID = Convert.ToInt32(Session["BrchID"]);

                IQueryable<tblUser> tblUser;

                if (branchTypeID == 1)  // Main Branch
                {
                    tblUser = from s in _db.tblUser
                              join sa in _db.tblEmployee on s.UserID equals sa.UserID
                              where sa.CompanyID == companyID
                              select s;

                    foreach (var item in tblUser)
                    {
                        item.FullName = item.FullName + "(" + _db.tblEmployee.FirstOrDefault(e => e.UserID == item.UserID)?.tblBranch.BranchName + ")";
                    }
                }
                else
                {
                    tblUser = from s in _db.tblUser
                              join sa in _db.tblEmployee on s.UserID equals sa.UserID
                              where sa.tblBranch.BrchID == brchID
                              select s;

                    foreach (var item in tblUser)
                    {
                        item.FullName = item.FullName + "(" + _db.tblEmployee.FirstOrDefault(e => e.UserID == item.UserID)?.tblBranch.BranchName + ")";
                    }
                }

                return View(tblUser.ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Details/5
        public ActionResult Details(int? id)
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Create
        public ActionResult Create()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                ViewBag.UserTypeID = new SelectList(_db.tblUserType, "UserTypeID", "UserType");
                
                return View(new tblUser());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: User/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(tblUser tblUser)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (ModelState.IsValid)
                {
                    int companyID = Convert.ToInt32(Session["CompanyID"]);
                    if (companyID == 0)
                    {
                        tblUser.UserTypeID = 1;
                    }
                    else
                    {
                        tblUser.UserTypeID = 2;
                    }

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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            try
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
                tblUser.Password = string.Empty;
                return View(tblUser);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: User/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. 
        // Дополнительные сведения см. в статье https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblUser tblUser)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (ModelState.IsValid)
                {
                    var currentUser = _db.tblUser.Find(tblUser.UserID);
                    if (currentUser == null)
                    {
                        return HttpNotFound();
                    }

                    if (!string.IsNullOrEmpty(tblUser.Password))
                    {
                        tblUser.Password = PasswordHelper.HashPassword(tblUser.Password, out string salt);
                        tblUser.Salt = salt;
                    }
                    else
                    {
                        tblUser.Password = currentUser.Password;
                        tblUser.Salt = currentUser.Salt;
                    }

                    currentUser.FullName = tblUser.FullName;
                    currentUser.Email = tblUser.Email;
                    currentUser.ContactNo = tblUser.ContactNo;
                    currentUser.UserName = tblUser.UserName;
                    currentUser.UserTypeID = tblUser.UserTypeID;
                    currentUser.IsActive = tblUser.IsActive;

                    _db.Entry(currentUser).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.UserTypeID = new SelectList(_db.tblUserType, "UserTypeID", "UserType", tblUser.UserTypeID);
                return View(tblUser);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
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