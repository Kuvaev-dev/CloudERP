using DatabaseAccess;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class UserSettingController : Controller
    {
        private readonly CloudDBEntities db = new CloudDBEntities();

        // GET: CreateUser
        public ActionResult CreateUser(int? employeeID)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            Session["CEmployeeID"] = employeeID;
            var employee = db.tblEmployee.Find(employeeID);
            var user = new tblUser
            {
                Email = employee.Email,
                ContactNo = employee.ContactNo,
                FullName = employee.Name,
                IsActive = true,
                Password = employee.ContactNo,
                UserName = employee.Email
            };
            ViewBag.UserTypeID = new SelectList(db.tblUserType.ToList(), "UserTypeID", "UserType");
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
                    var user = db.tblUser.Where(u => u.Email == tblUser.Email && u.UserID != tblUser.UserID);
                    if (user.Count() > 0)
                    {
                        ViewBag.Message = "Email is Already Registered";
                    }
                    else
                    {
                        db.tblUser.Add(tblUser);
                        db.SaveChanges();
                        int? employeeID = Convert.ToInt32(Convert.ToString(Session["CEmployeeID"]));
                        var employee = db.tblEmployee.Find(employeeID);
                        employee.UserID = tblUser.UserID;
                        db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        Session["CEmployeeID"] = null;
                        return RedirectToAction("Index", "User");
                    }
                }

                if (tblUser == null)
                {
                    ViewBag.UserTypeID = new SelectList(db.tblUserType.ToList(), "UserTypeID", "UserType");
                }
                else
                {
                    ViewBag.UserTypeID = new SelectList(db.tblUserType.ToList(), "UserTypeID", "UserType", tblUser.UserTypeID);
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
            var user = db.tblUser.Find(userID);
            ViewBag.UserTypeID = new SelectList(db.tblUserType.ToList(), "UserTypeID", "UserType", user.UserTypeID);
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
                var user = db.tblUser.Where(u => u.Email == tblUser.Email && u.UserID != tblUser.UserID);
                if (user.Count() > 0)
                {
                    ViewBag.Message = "Email is Already Registered";
                }
                else
                {
                    db.Entry(tblUser).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index", "User");
                }
            }

            if (tblUser == null)
            {
                ViewBag.UserTypeID = new SelectList(db.tblUserType.ToList(), "UserTypeID", "UserType");
            }
            else
            {
                ViewBag.UserTypeID = new SelectList(db.tblUserType.ToList(), "UserTypeID", "UserType", tblUser.UserTypeID);
            }

            return View(tblUser);
        }
    }
}