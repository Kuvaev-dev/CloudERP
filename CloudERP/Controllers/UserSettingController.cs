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
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                Session["CEmployeeID"] = employeeID;
                var employee = _db.tblEmployee.Find(employeeID);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = Resources.Messages.EmployeeNotFound;
                    return RedirectToAction("EP500", "EP");
                }

                string salt;
                var hashedPassword = PasswordHelper.HashPassword(employee.ContactNo, out salt);

                var user = new tblUser
                {
                    Email = employee.Email,
                    ContactNo = employee.ContactNo,
                    FullName = employee.Name,
                    IsActive = true,
                    Password = hashedPassword,
                    Salt = salt,
                    UserName = employee.Email
                };

                ViewBag.Password = hashedPassword;
                ViewBag.Salt = salt;

                ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType");

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: CreateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateUser(tblUser tblUser)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (ModelState.IsValid)
                {
                    var existingUser = _db.tblUser.FirstOrDefault(u => u.Email == tblUser.Email && u.UserID != tblUser.UserID);
                    if (existingUser != null)
                    {
                        ViewBag.Message = Resources.Messages.EmailIsAlreadyRegistered;
                    }
                    else
                    {
                        string password = Request.Form["Password"];
                        string salt = Request.Form["Salt"];

                        tblUser.Password = password;
                        tblUser.Salt = salt;

                        _db.tblUser.Add(tblUser);
                        _db.SaveChanges();

                        int? employeeID = Convert.ToInt32(Session["CEmployeeID"]);
                        var employee = _db.tblEmployee.Find(employeeID);
                        if (employee != null)
                        {
                            employee.UserID = tblUser.UserID;
                            _db.Entry(employee).State = System.Data.Entity.EntityState.Modified;
                            _db.SaveChanges();
                        }
                        Session["CEmployeeID"] = null;

                        return RedirectToAction("Index", "User");
                    }
                }

                ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType", tblUser.UserTypeID);

                return View(tblUser);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: UpdateUser
        public ActionResult UpdateUser(int? userID)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var user = _db.tblUser.Find(userID);
                if (user == null)
                {
                    TempData["ErrorMessage"] = Resources.Messages.UserNotFound;
                    return RedirectToAction("EP500", "EP");
                }

                ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType", user.UserTypeID);

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: UpdateUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUser(tblUser tblUser)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                if (ModelState.IsValid)
                {
                    var existingUser = _db.tblUser.FirstOrDefault(u => u.Email == tblUser.Email && u.UserID != tblUser.UserID);
                    if (existingUser != null)
                    {
                        ViewBag.Message = Resources.Messages.EmailIsAlreadyRegistered;
                    }
                    else
                    {
                        _db.Entry(tblUser).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();

                        return RedirectToAction("Index", "User");
                    }
                }

                ViewBag.UserTypeID = new SelectList(_db.tblUserType.ToList(), "UserTypeID", "UserType", tblUser.UserTypeID);

                return View(tblUser);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}