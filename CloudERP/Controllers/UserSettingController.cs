using CloudERP.Helpers;
using DatabaseAccess;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class UserSettingController : Controller
    {
        private readonly CloudDBEntities _db;

        public UserSettingController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: CreateUser
        public ActionResult CreateUser(int? employeeID)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            Session["CEmployeeID"] = employeeID;
            var employee = _db.tblEmployee.Find(employeeID);
            byte[] salt;
            var user = new tblUser
            {
                Email = employee.Email,
                ContactNo = employee.ContactNo,
                FullName = employee.Name,
                IsActive = true,
                Password = PasswordHelper.HashPassword(employee.ContactNo, out salt),
                Salt = Convert.ToBase64String(salt),
                UserName = employee.Email
            };
            ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType");
            return View(user);
        }

        // POST: CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(tblUser tblUser)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _db.tblUser.Where(u => u.Email == tblUser.Email && u.UserID != tblUser.UserID);
                    if (user.Count() > 0)
                    {
                        ViewBag.Message = "Email is Already Registered";
                    }
                    else
                    {
                        _db.tblUser.Add(tblUser);
                        _db.SaveChanges();
                        int? employeeID = Convert.ToInt32(Convert.ToString(Session["CEmployeeID"]));
                        var employee = _db.tblEmployee.Find(employeeID);
                        employee.UserID = tblUser.UserID;
                        _db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                        Session["CEmployeeID"] = null;
                        return RedirectToAction("Index", "User");
                    }
                }

                if (tblUser == null)
                {
                    ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType");
                }
                else
                {
                    ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType", tblUser.UserTypeID);
                }

                return View(tblUser);
            }
            catch
            {
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: UpdateUser
        public ActionResult UpdateUser(int? userID)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            var user = _db.tblUser.Find(userID);
            ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType", user.UserTypeID);
            return View(user);
        }

        // GET: UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(tblUser tblUser)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                var user = _db.tblUser.Where(u => u.Email == tblUser.Email && u.UserID != tblUser.UserID);
                if (user.Count() > 0)
                {
                    ViewBag.Message = "Email is Already Registered";
                }
                else
                {
                    _db.Entry(tblUser).State = System.Data.Entity.EntityState.Modified;
                    _db.SaveChanges();
                    return RedirectToAction("Index", "User");
                }
            }

            if (tblUser == null)
            {
                ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType");
            }
            else
            {
                ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType", tblUser.UserTypeID);
            }

            return View(tblUser);
        }
    }
}