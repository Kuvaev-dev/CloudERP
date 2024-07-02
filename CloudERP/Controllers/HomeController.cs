using CloudERP.Helpers;
using DatabaseAccess;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
            try
            {
                var user = _db.tblUser.SingleOrDefault(u => u.Email == email);
                if (user != null)
                {
                    byte[] saltBytes = Convert.FromBase64String(user.Salt);

                    if (PasswordHelper.VerifyPassword(password, user.Password, saltBytes))
                    {
                        FormsAuthentication.SetAuthCookie(user.Email, false);

                        if (rememberMe.HasValue && rememberMe.Value)
                        {
                            HttpCookie cookie = new HttpCookie("RememberMe");
                            cookie.Values["Email"] = email;
                            cookie.Expires = DateTime.Now.AddDays(30);
                            Response.Cookies.Add(cookie);
                        }
                        else
                        {
                            if (Request.Cookies["RememberMe"] != null)
                            {
                                HttpCookie cookie = new HttpCookie("RememberMe");
                                cookie.Expires = DateTime.Now.AddDays(-1);
                                Response.Cookies.Add(cookie);
                            }
                        }

                        return RedirectToAction("Index", "Home");
                    }
                }

                ViewBag.Message = "Invalid credentials. Please try again.";
                return View("Login");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error logging in: " + ex.Message;
                return View("Error");
            }
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
            try
            {
                ResourceManagerHelper.SetCulture(culture);
                Session["Culture"] = culture;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            try
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
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
            try
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                return View("ResetPasswordLinkExpired");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(string id, string newPassword, string confirmPassword)
        {
            try
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
                    catch (Exception ex)
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
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while processing your request. Please try again later.";
                return View("ResetPasswordLinkExpired");
            }
        }
    }
}