using CloudERP.Helpers;
using DatabaseAccess;
using DatabaseAccess.Models;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;

namespace CloudERP.Controllers
{
    public class HomeController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_Dashboard _spDashboard;
        private readonly SessionHelper _sessionHelper;

        public HomeController(CloudDBEntities db, SP_Dashboard spDashboard, SessionHelper sessionHelper)
        {
            _db = db;
            _spDashboard = spDashboard;
            _sessionHelper = sessionHelper;
        }

        public ActionResult Index()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login");
                }

                DateTime currentDate = DateTime.Today;
                string fromDate = new DateTime(currentDate.Year, currentDate.Month, 1).ToString("yyyy-MM-dd");
                string toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month)).ToString("yyyy-MM-dd");

                DashboardModel dashboardValues = _spDashboard.GetDashboardValues(fromDate, toDate, _sessionHelper.BranchID, _sessionHelper.CompanyID);

                return View(dashboardValues);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult Login()
        {
            var rememberMeCookie = Request.Cookies["RememberMe"];
            if (rememberMeCookie != null && !string.IsNullOrEmpty(rememberMeCookie["Email"]))
            {
                ViewBag.RememberedEmail = rememberMeCookie["Email"];
            }
            else
            {
                ViewBag.RememberedEmail = string.Empty;
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
                    bool isPasswordValid = PasswordHelper.VerifyPassword(password, user.Password, user.Salt);
                    if (isPasswordValid)
                    {
                        FormsAuthentication.SetAuthCookie(user.Email, false);

                        if (rememberMe.HasValue && rememberMe.Value)
                        {
                            var cookie = new HttpCookie("RememberMe")
                            {
                                Values = { ["Email"] = email },
                                Expires = DateTime.Now.AddDays(30),
                                HttpOnly = true
                            };
                            Response.Cookies.Add(cookie);
                        }
                        else
                        {
                            var cookie = new HttpCookie("RememberMe")
                            {
                                Expires = DateTime.Now.AddDays(-1)
                            };
                            Response.Cookies.Add(cookie);
                        }

                        Session["UserID"] = user.UserID;
                        Session["UserTypeID"] = user.UserTypeID;
                        Session["FullName"] = user.FullName;
                        Session["Email"] = user.Email;
                        Session["ContactNo"] = user.ContactNo;
                        Session["UserName"] = user.UserName;
                        Session["Password"] = user.Password;
                        Session["Salt"] = user.Salt;
                        Session["IsActive"] = user.IsActive;

                        var employee = _db.tblEmployee.Where(e => e.UserID == user.UserID).FirstOrDefault();
                        if (employee == null)
                        {
                            ClearSession();
                            return RedirectToAction("Login", "Home");
                        }

                        Session["EName"] = employee.Name;
                        Session["EPhoto"] = employee.Photo;
                        Session["ERegistrationDate"] = employee.RegistrationDate;
                        Session["Designation"] = employee.Designation;
                        Session["BranchID"] = employee.BranchID;
                        Session["BranchTypeID"] = employee.tblBranch.BranchTypeID;
                        Session["BrchID"] = employee.tblBranch.BrchID;
                        Session["CompanyID"] = employee.CompanyID;

                        var company = _db.tblCompany.Where(c => c.CompanyID == employee.CompanyID).FirstOrDefault();
                        if (company == null)
                        {
                            ClearSession();
                            return RedirectToAction("Login", "Home");
                        }

                        Session["CName"] = company.Name;
                        Session["CLogo"] = company.Logo;

                        if ((bool)employee.IsFirstLogin)
                        {
                            Session["StartTour"] = true;
                            employee.IsFirstLogin = false;
                            _db.SaveChanges();
                        }

                        if (user.UserTypeID == 1)
                        {
                            return RedirectToAction("AdminMenuGuide", "Guide");
                        }
                        else if (user.UserTypeID == 2)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                ViewBag.Message = Resources.Messages.PleaseProvideCorrectDetails;
                return View("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return View("Login");
            }
        }

        private void ClearSession()
        {
            Session.Clear();
            Session.Abandon();
        }

        public ActionResult Logout()
        {
            ClearSession();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: ForgotPassword
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", Resources.Messages.EmailIsRequired);
                return View();
            }

            try
            {
                var user = _db.tblUser.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    if (user.LastPasswordResetRequest.HasValue && (DateTime.Now - user.LastPasswordResetRequest.Value).TotalMinutes < 5)
                    {
                        ModelState.AddModelError("", Resources.Messages.PasswordResetAlreadyRequested);
                        return View();
                    }

                    user.ResetPasswordCode = Guid.NewGuid().ToString();
                    user.ResetPasswordExpiration = DateTime.Now.AddHours(1);
                    user.LastPasswordResetRequest = DateTime.Now;

                    _db.Entry(user).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    try
                    {
                        SendPasswordResetEmail(user.Email, user.ResetPasswordCode);
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                        return RedirectToAction("EP500", "EP");
                    }

                    return View("ForgotPasswordEmailSent");
                }
                else
                {
                    return View("ForgotPasswordEmailSent");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private void SendPasswordResetEmail(string email, string resetCode)
        {
            try
            {
                var resetLink = Url.Action("ResetPassword", "Home", new { id = resetCode }, protocol: Request.Url.Scheme);
                var subject = "Password Reset";
                var body = $"<strong>Please reset your password by clicking the following link: <a href='{resetLink}'>Reset Password</a></strong>";

                var emailService = new EmailService();
                emailService.SendEmail(email, subject, body);
            }
            catch (Exception ex)
            {
                throw new Exception(Resources.Messages.UnexpectedErrorMessage + ex.Message);
            }
        }

        // GET: ResetPassword
        public ActionResult ResetPassword(string id)
        {
            try
            {
                var user = _db.tblUser.FirstOrDefault(u => u.ResetPasswordCode == id && u.ResetPasswordExpiration > DateTime.Now);
                if (user != null)
                {
                    ViewBag.ResetCode = id;
                    return View();
                }
                else
                {
                    return View("ResetPasswordLinkExpired");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string id, string newPassword, string confirmPassword)
        {
            try
            {
                if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("", "Passwords do not match.");
                    return View();
                }

                var user = _db.tblUser.FirstOrDefault(u => u.ResetPasswordCode == id && u.ResetPasswordExpiration > DateTime.Now);
                if (user != null)
                {
                    user.Password = PasswordHelper.HashPassword(newPassword, out string salt);
                    user.Salt = salt;
                    user.ResetPasswordCode = null;
                    user.ResetPasswordExpiration = null;
                    user.LastPasswordResetRequest = null;

                    try
                    {
                        _db.Entry(user).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                        return RedirectToAction("EP500", "EP");
                    }

                    return View("ResetPasswordSuccess");
                }
                else
                {
                    return View("ResetPasswordLinkExpired");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}