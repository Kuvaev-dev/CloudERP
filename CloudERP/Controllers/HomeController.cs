using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class HomeController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login");
            }

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginUser(string email, string password)
        {
            var user = db.tblUser.Where(u => u.Email == email && u.Password == password && u.IsActive == true).FirstOrDefault();
            if (user != null)
            {
                Session["UserID"] = user.UserID;
                Session["UserTypeID"] = user.UserTypeID;
                Session["FullName"] = user.FullName;
                Session["Email"] = user.Email;
                Session["ContactNo"] = user.ContactNo;
                Session["UserName"] = user.UserName;
                Session["Password"] = user.Password;
                Session["IsActive"] = user.IsActive;

                var employeeDetails = db.tblEmployee.Where(e => e.UserID == user.UserID).FirstOrDefault();
                if (employeeDetails == null)
                {
                    ViewBag.Message = "Please contact to Administrator";

                    Session["UserTypeID"] = string.Empty;
                    Session["FullName"] = string.Empty;
                    Session["Email"] = string.Empty;
                    Session["ContactNo"] = string.Empty;
                    Session["UserName"] = string.Empty;
                    Session["Password"] = string.Empty;
                    Session["IsActive"] = string.Empty;
                    Session["EmployeeID"] = string.Empty;
                    Session["EName"] = string.Empty;
                    Session["EPhoto"] = string.Empty;
                    Session["Designation"] = string.Empty;
                    Session["BranchID"] = string.Empty;
                    Session["CompanyID"] = string.Empty;

                    return View("Login");
                }

                Session["EmployeeID"] = employeeDetails.EmployeeID;
                Session["EName"] = employeeDetails.Name;
                Session["EPhoto"] = employeeDetails.Photo;
                Session["Designation"] = employeeDetails.Designation;
                Session["BranchID"] = employeeDetails.BranchID;
                Session["CompanyID"] = employeeDetails.CompanyID;

                var company = db.tblCompany.Where(c => c.CompanyID == employeeDetails.CompanyID).FirstOrDefault();
                if (company == null)
                {
                    ViewBag.Message = "Please contact to Administrator";

                    Session["UserTypeID"] = string.Empty;
                    Session["FullName"] = string.Empty;
                    Session["Email"] = string.Empty;
                    Session["ContactNo"] = string.Empty;
                    Session["UserName"] = string.Empty;
                    Session["Password"] = string.Empty;
                    Session["IsActive"] = string.Empty;
                    Session["EmployeeID"] = string.Empty;
                    Session["EName"] = string.Empty;
                    Session["EPhoto"] = string.Empty;
                    Session["Designation"] = string.Empty;
                    Session["BranchID"] = string.Empty;
                    Session["CompanyID"] = string.Empty;

                    return View("Login");
                }

                Session["CName"] = company.Name;
                Session["CLogo"] = company.Logo;

                var branchType = db.tblBranch.Where(b => b.BranchID == employeeDetails.BranchID).FirstOrDefault();
                if (branchType == null)
                {
                    ViewBag.Message = "Please contact to Administrator";
                    return View("Login");
                }
                Session["BranchTypeID"] = branchType.BranchTypeID;
                Session["BrchID"] = branchType.BrchID == null ? 0 : branchType.BrchID;

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Incorrect credentials";

                Session["UserTypeID"] = string.Empty;
                Session["FullName"] = string.Empty;
                Session["Email"] = string.Empty;
                Session["ContactNo"] = string.Empty;
                Session["UserName"] = string.Empty;
                Session["Password"] = string.Empty;
                Session["IsActive"] = string.Empty;
                Session["EmployeeID"] = string.Empty;
                Session["EName"] = string.Empty;
                Session["EPhoto"] = string.Empty;
                Session["Designation"] = string.Empty;
                Session["BranchID"] = string.Empty;
                Session["CompanyID"] = string.Empty;
                Session["BrchID"] = string.Empty;
            }

            return View("Login");
        }

        public ActionResult Logout()
        {
            Session["UserTypeID"] = string.Empty;
            Session["FullName"] = string.Empty;
            Session["Email"] = string.Empty;
            Session["ContactNo"] = string.Empty;
            Session["UserName"] = string.Empty;
            Session["Password"] = string.Empty;
            Session["IsActive"] = string.Empty;
            Session["EmployeeID"] = string.Empty;
            Session["EName"] = string.Empty;
            Session["EPhoto"] = string.Empty;
            Session["Designation"] = string.Empty;
            Session["BranchID"] = string.Empty;
            Session["CompanyID"] = string.Empty;
            Session["BrchID"] = string.Empty;

            return View("Login");
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}