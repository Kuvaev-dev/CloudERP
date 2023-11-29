using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
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
            var rememberMeCookie = Request.Cookies["RememberMe"];
            if (rememberMeCookie != null)
            {
                ViewBag.RememberedEmail = rememberMeCookie["Email"];
            }
            return View();
        }

        [HttpPost]
        public ActionResult LoginUser(string email, string password, bool rememberMe)
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

                if (Convert.ToInt32(Convert.ToString(Session["UserTypeID"])) == 1)  // Admin
                {
                    return RedirectToAction("AdminMenuGuide", "Guide");
                }
                else if (Convert.ToInt32(Convert.ToString(Session["UserTypeID"])) == 2) // User
                {
                    return RedirectToAction("Index");
                }

                if (rememberMe)
                {
                    HttpCookie cookie = new HttpCookie("RememberMe");
                    cookie.Values["Email"] = email;
                    cookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(cookie);
                }
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

        [HttpPost]
        public async Task<ActionResult> ForgetPassword(string email)
        {
            var user = db.tblUser.FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                ViewBag.Message = "User With Provided E-mail is Not Found";
                return View();
            }
            var resetCode = Guid.NewGuid().ToString();
            user.ResetPasswordCode = resetCode;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            var resetLink = Url.Action("ResetPassword", "Home", new { id = resetCode }, protocol: Request.Url.Scheme);

            var message = new MailMessage();
            message.To.Add(user.Email);
            message.Subject = "Password Reset";
            message.Body = "To Reset Your Password Please Follow This Link: " + resetLink;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                await smtp.SendMailAsync(message);
            }

            return RedirectToAction("ForgotPasswordConfirmation", "Home");
        }

        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            var user = db.tblUser.FirstOrDefault(u => u.ResetPasswordCode == id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new ResetPasswordModel { ResetCode = id };
            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var user = db.tblUser.FirstOrDefault(u => u.ResetPasswordCode == model.ResetCode);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.Password = model.NewPassword;
            user.ResetPasswordCode = null;

            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("ResetPasswordConfirmation", "Home");
        }

        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);

            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture")
                {
                    Value = culture,
                    Expires = DateTime.Now.AddYears(1)
                };
            }
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index");
        }
    }
}