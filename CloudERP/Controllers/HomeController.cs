using CloudERP.Helpers;
using DatabaseAccess;
using DatabaseAccess.Models;
using DatabaseAccess.Code.SP_Code;
using SendGrid;
using SendGrid.Helpers.Mail;
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
        private readonly ExchangeRateService _exchangeRateService;

        public HomeController(CloudDBEntities db)
        {
            _db = db;
            _spDashboard = new SP_Dashboard(_db);
            _exchangeRateService = new ExchangeRateService(System.Configuration.ConfigurationManager.AppSettings["ExchangeRateApiKey"]);
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            return await Index(System.Configuration.ConfigurationManager.AppSettings["DefaultCurrency"]);
        }

        [HttpPost]
        public async Task<ActionResult> Index(string currency)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                DateTime currentDate = DateTime.Today;
                string fromDate = new DateTime(currentDate.Year, currentDate.Month, 1).ToString("yyyy-MM-dd");
                string toDate = new DateTime(currentDate.Year, currentDate.Month, DateTime.DaysInMonth(currentDate.Year, currentDate.Month)).ToString("yyyy-MM-dd");

                DashboardModel dashboardValues = _spDashboard.GetDashboardValues(fromDate, toDate, branchID, companyID);

                var rates = await _exchangeRateService.GetExchangeRatesAsync();
                dashboardValues.CurrencyRates = rates;
                dashboardValues.SelectedCurrency = currency;

                if (rates.TryGetValue(System.Configuration.ConfigurationManager.AppSettings["DefaultCurrency"], out double defaultRate))
                {
                    dashboardValues.DefaultCurrencyRate = defaultRate;

                    if (rates.TryGetValue(currency, out double selectedRate))
                    {
                        dashboardValues.SelectedCurrencyRate = selectedRate;
                        dashboardValues.ConversionRateToDefault = selectedRate;
                        dashboardValues.ConversionRateFromDefault = 1 / selectedRate;
                        dashboardValues.CurrentMonthRecovery *= (selectedRate / defaultRate);
                    }
                }

                return View(dashboardValues);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
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

                ViewBag.Message = "Invalid credentials. Please try again.";
                return View("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred: " + ex.Message;
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
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
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
                ModelState.AddModelError("", "Email is required.");
                return View();
            }

            try
            {
                var user = _db.tblUser.FirstOrDefault(u => u.Email == email);
                if (user != null)
                {
                    if (user.LastPasswordResetRequest.HasValue && (DateTime.Now - user.LastPasswordResetRequest.Value).TotalMinutes < 5)
                    {
                        ModelState.AddModelError("", "You have already requested a password reset. Please wait a while before trying again.");
                        return View();
                    }

                    user.ResetPasswordCode = Guid.NewGuid().ToString();
                    user.ResetPasswordExpiration = DateTime.Now.AddHours(1);
                    user.LastPasswordResetRequest = DateTime.Now;

                    _db.Entry(user).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    try
                    {
                        await SendPasswordResetEmail(user.Email, user.ResetPasswordCode);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An error occurred while sending the email. Please try again.");
                        return View();
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
                TempData["ErrorMessage"] = "An unexpected error occurred: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task SendPasswordResetEmail(string email, string resetCode)
        {
            try
            {
                var resetLink = Url.Action("ResetPassword", "Home", new { id = resetCode }, protocol: Request.Url.Scheme);

                var apiKey = System.Configuration.ConfigurationManager.AppSettings["SendGridApiKey"];
                var fromEmail = System.Configuration.ConfigurationManager.AppSettings["SendGridFromEmail"];
                var fromName = System.Configuration.ConfigurationManager.AppSettings["SendGridFromName"];

                var client = new SendGridClient(apiKey);
                var from = new EmailAddress(fromEmail, fromName);
                var subject = "Password Reset";
                var to = new EmailAddress(email);
                var plainTextContent = $"Please reset your password by clicking the following link: {resetLink}";
                var htmlContent = $"<strong>Please reset your password by clicking the following link: <a href='{resetLink}'>Reset Password</a></strong>";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending email: " + ex.Message);
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
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
                return View("ResetPasswordLinkExpired");
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
                    catch (Exception)
                    {
                        ModelState.AddModelError("", "An error occurred while saving to the database. Please try again.");
                        return View();
                    }

                    return View("ResetPasswordSuccess");
                }
                else
                {
                    return View("ResetPasswordLinkExpired");
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while processing your request. Please try again later.";
                return View("ResetPasswordLinkExpired");
            }
        }
    }
}