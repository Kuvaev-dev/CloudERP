using CloudERP.Helpers;
using DatabaseAccess;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Data.Entity;
using System.Linq;
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
            var user = _db.tblUser.FirstOrDefault(u => u.Email == email && u.IsActive);
            if (user != null)
            {
                var saltBytes = Convert.FromBase64String(user.Salt);
                if (PasswordHelper.VerifyPassword(password, user.Password, saltBytes))
                {
                    Session["UserID"] = user.UserID;
                    Session["UserTypeID"] = user.UserTypeID;
                    Session["FullName"] = user.FullName;
                    Session["Email"] = user.Email;
                    Session["ContactNo"] = user.ContactNo;
                    Session["UserName"] = user.UserName;
                    Session["IsActive"] = user.IsActive;

                    var employeeDetails = _db.tblEmployee.FirstOrDefault(e => e.UserID == user.UserID);
                    if (employeeDetails == null)
                    {
                        ViewBag.Message = "Please contact the Administrator";
                        ClearSession();
                        return View("Login");
                    }

                    Session["EmployeeID"] = employeeDetails.EmployeeID;
                    Session["EName"] = employeeDetails.Name;
                    Session["EPhoto"] = employeeDetails.Photo;
                    Session["Designation"] = employeeDetails.Designation;
                    Session["BranchID"] = employeeDetails.BranchID;
                    Session["CompanyID"] = employeeDetails.CompanyID;

                    var company = _db.tblCompany.FirstOrDefault(c => c.CompanyID == employeeDetails.CompanyID);
                    if (company == null)
                    {
                        ViewBag.Message = "Please contact the Administrator";
                        ClearSession();
                        return View("Login");
                    }

                    Session["CName"] = company.Name;
                    Session["CLogo"] = company.Logo;

                    var branchType = _db.tblBranch.FirstOrDefault(b => b.BranchID == employeeDetails.BranchID);
                    if (branchType == null)
                    {
                        ViewBag.Message = "Please contact the Administrator";
                        return View("Login");
                    }
                    Session["BranchTypeID"] = branchType.BranchTypeID;
                    Session["BrchID"] = branchType.BrchID ?? 0;

                    if (Convert.ToInt32(Session["UserTypeID"]) == 1)  // Admin
                    {
                        return RedirectToAction("AdminMenuGuide", "Guide");
                    }
                    else if (Convert.ToInt32(Session["UserTypeID"]) == 2) // User
                    {
                        return RedirectToAction("Index");
                    }

                    if (rememberMe.HasValue && rememberMe.Value)
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
                    ClearSession();
                }
            }
            else
            {
                ViewBag.Message = "Incorrect credentials";
                ClearSession();
            }

            return View("Login");
        }

        private void ClearSession()
        {
            Session["UserTypeID"] = string.Empty;
            Session["FullName"] = string.Empty;
            Session["Email"] = string.Empty;
            Session["ContactNo"] = string.Empty;
            Session["UserName"] = string.Empty;
            Session["IsActive"] = string.Empty;
            Session["EmployeeID"] = string.Empty;
            Session["EName"] = string.Empty;
            Session["EPhoto"] = string.Empty;
            Session["Designation"] = string.Empty;
            Session["BranchID"] = string.Empty;
            Session["CompanyID"] = string.Empty;
            Session["BrchID"] = string.Empty;
        }

        public ActionResult Logout()
        {
            ClearSession();
            return View("Login");
        }

        public ActionResult SetCulture(string culture)
        {
            ResourceManagerHelper.SetCulture(culture);
            Session["Culture"] = culture;
            return Redirect(Request.UrlReferrer.ToString());
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
                catch (Exception)
                {
                    ModelState.AddModelError("", "Произошла ошибка при сохранении в базу данных. Пожалуйста, попробуйте еще раз.");
                    return View("ForgotPassword");
                }

                try
                {
                    SendPasswordResetEmail(user.Email, user.ResetPasswordCode);
                }
                catch (Exception)
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
            var htmlContent = $"<strong>Пожалуйста, сбросьте свой пароль, перейдя по следующей ссылке: <a href='{resetLink}'>Сбросить пароль</a></strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg).Result;
        }

        public ActionResult ResetPassword(string id)
        {
            var user = _db.tblUser.FirstOrDefault(u => u.ResetPasswordCode == id && u.ResetPasswordExpiration > DateTime.Now);
            if (user != null)
            {
                return View();
            }
            else
            {
                return View("ResetPasswordLinkExpired");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(string id, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ModelState.AddModelError("", "Пароли не совпадают.");
                return View();
            }

            var user = _db.tblUser.FirstOrDefault(u => u.ResetPasswordCode == id && u.ResetPasswordExpiration > DateTime.Now);
            if (user != null)
            {
                byte[] salt;
                user.Password = PasswordHelper.HashPassword(newPassword, out salt);
                user.Salt = Convert.ToBase64String(salt);

                user.ResetPasswordCode = null;
                user.ResetPasswordExpiration = null;

                try
                {
                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();
                    return View("ResetPasswordSuccess");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Произошла ошибка при сохранении в базу данных. Пожалуйста, попробуйте еще раз.");
                    return View();
                }
            }
            else
            {
                return View("ResetPasswordLinkExpired");
            }
        }
    }
}
