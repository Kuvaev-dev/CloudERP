using CloudERP.Helpers;
using DatabaseAccess;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class HomeController : Controller
    {
        private readonly CloudDBEntities _db;

        public HomeController(CloudDBEntities db)
        {
            _db = db;
        }

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
        public ActionResult LoginUser(string email, string password, bool? rememberMe)
        {
            var user = _db.tblUser.Where(u => u.Email == email && u.Password == password && u.IsActive == true).FirstOrDefault();
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

                var employeeDetails = _db.tblEmployee.Where(e => e.UserID == user.UserID).FirstOrDefault();
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

                var company = _db.tblCompany.Where(c => c.CompanyID == employeeDetails.CompanyID).FirstOrDefault();
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

                var branchType = _db.tblBranch.Where(b => b.BranchID == employeeDetails.BranchID).FirstOrDefault();
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

                if (rememberMe.HasValue && rememberMe.Value)
                {
                    HttpCookie cookie = new HttpCookie("rememberMe");
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

        public ActionResult SetCulture(string culture)
        {
            ResourceManagerHelper.SetCulture(culture);
            if (Session["UserTypeID"] != null)
            {
                int userTypeID = Convert.ToInt32(Session["UserTypeID"]);
                if (userTypeID == 1) // Admin
                {
                    return RedirectToAction("AdminMenuGuide", "Guide");
                }
                else if (userTypeID == 2) // User
                {
                    return RedirectToAction("Index");
                }
            }

            return View("Login");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var user = _db.tblUser.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                if (user.LastPasswordResetRequest.HasValue && (DateTime.Now - user.LastPasswordResetRequest.Value).TotalMinutes < 5)
                {
                    ModelState.AddModelError("", "Вы уже запрашивали сброс пароля. Пожалуйста, подождите некоторое время, прежде чем делать это снова.");
                    return View("ForgotPassword");
                }

                user.ResetPasswordCode = Guid.NewGuid().ToString();
                user.ResetPasswordExpiration = DateTime.Now.AddHours(1);
                user.LastPasswordResetRequest = DateTime.Now;

                try
                {
                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Обработка ошибок при сохранении в базу данных
                    ModelState.AddModelError("", "Произошла ошибка при сохранении в базу данных. Пожалуйста, попробуйте еще раз.");
                    return View("ForgotPassword");
                }

                try
                {
                    SendPasswordResetEmail(user.Email, user.ResetPasswordCode);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Произошла ошибка при отправке электронной почты. Пожалуйста, попробуйте еще раз.");
                    return View("ForgotPassword");
                }

                return View("ForgotPasswordEmailSent");
            }
            else
            {
                ModelState.AddModelError("", "Адрес электронной почты не найден.");
                return View("ForgotPassword");
            }
        }

        private void SendPasswordResetEmail(string email, string resetCode)
        {
            var resetLink = Url.Action("ResetPassword", "Home", new { id = resetCode }, protocol: Request.Url.Scheme);

            var client = new SendGridClient("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");
            var from = new EmailAddress("kuvaevtestmail@gmail.com", "Cloud ERP");
            var subject = "Сброс пароля";
            var to = new EmailAddress(email);
            var plainTextContent = $"Пожалуйста, сбросьте свой пароль, перейдя по следующей ссылке: {resetLink}";
            var htmlContent = $"<p>Пожалуйста, сбросьте свой пароль, перейдя по следующей ссылке: <a href='{resetLink}'>ссылка</a></p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            client.SendEmailAsync(msg);
        }

        public ActionResult ResetPassword(string id)
        {
            var user = _db.tblUser.FirstOrDefault(u => u.ResetPasswordCode == id);
            if (user != null)
            {
                return View();
            }
            else
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(string id, string password, string confirmPassword)
        {
            var user = _db.tblUser.FirstOrDefault(u => u.ResetPasswordCode == id);
            if (user != null)
            {
                if (password == confirmPassword)
                {
                    user.Password = password;
                    user.ResetPasswordCode = null;

                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();

                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Пароли не совпадают.");
                    return View();
                }
            }
            else
            {
                return HttpNotFound();
            }
        }
    }
}