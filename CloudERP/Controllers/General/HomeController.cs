using CloudERP.Helpers;
using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using Utils.Helpers;
using Domain.Models.FinancialModels;
using Domain.Models;
using CloudERP.Models;
using System.Web.Http;

namespace CloudERP.Controllers
{
    public class HomeController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly ResourceManagerHelper _resourceManagerHelper;
        private readonly HttpClientHelper _httpClient;

        private const int ADMIN_USER_TYPE_ID = 1;

        public HomeController( 
            SessionHelper sessionHelper, 
            ResourceManagerHelper resourceManagerHelper,
            HttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _resourceManagerHelper = resourceManagerHelper ?? throw new ArgumentNullException(nameof(ResourceManagerHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            try
            {
                var dashboardValues = await _httpClient.GetAsync<DashboardModel>($"home/dashboard?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}");
                var currencies = await _httpClient.GetAsync<Dictionary<string, decimal>>("home/currencies");

                ViewBag.Currencies = currencies;
                ViewBag.SelectedCurrency = Session["SelectedCurrency"] as string ?? "UAH";

                return View(dashboardValues);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult Login()
        {
            ViewBag.RememberedEmail = Request.Cookies["RememberMe"]?["Email"] ?? string.Empty;
            return View(new LoginRequest() { RememberMe = false });
        }

        [System.Web.Http.HttpPost]
        public async Task<ActionResult> LoginUser([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var user = await _httpClient.PostAsync<User>("home/login", loginRequest);
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(user.Email, false);

                    Session["UserID"] = user.UserID;
                    Session["UserTypeID"] = user.UserTypeID;
                    Session["FullName"] = user.FullName;
                    Session["Email"] = user.Email;
                    Session["ContactNo"] = user.ContactNo;
                    Session["UserName"] = user.UserName;
                    Session["Password"] = user.Password;
                    Session["Salt"] = user.Salt;
                    Session["IsActive"] = user.IsActive;

                    var employee = await _httpClient.PostAsync<Employee>("home/employee", new { user.UserID });
                    if (employee == null)
                    {
                        ClearSession();
                        return RedirectToAction("Login", "Home");
                    }

                    Session["EName"] = employee.FullName;
                    Session["EPhoto"] = employee.Photo;
                    Session["ERegistrationDate"] = employee.RegistrationDate;
                    Session["Designation"] = employee.Designation;
                    Session["BranchID"] = employee.BranchID;
                    Session["BranchTypeID"] = employee.BranchTypeID;
                    Session["BrchID"] = employee.BrchID;
                    Session["CompanyID"] = employee.CompanyID;

                    var company = await _httpClient.PostAsync<Company>("home/company", new { employee.CompanyID });
                    if (company == null)
                    {
                        ClearSession();
                        return RedirectToAction("Login", "Home");
                    }

                    Session["CName"] = company.Name;
                    Session["CLogo"] = company.Logo;

                    var isFirstLogin = await _httpClient.PostAsync<bool>("home/isFirstLogin", new { employee.UserID });
                    if (isFirstLogin)
                    {
                        Session["StartTour"] = true;
                    }

                    var currencies = await _httpClient.GetAsync<Dictionary<string, decimal>>("home/currencies");
                    ViewBag.Currencies = currencies;

                    return user.UserTypeID == ADMIN_USER_TYPE_ID ? RedirectToAction("AdminMenuGuide", "Guide") : RedirectToAction("Index", "Home");
                }

                ViewBag.Message = Localization.CloudERP.Messages.Messages.PleaseProvideCorrectDetails;
                return View("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                _resourceManagerHelper.SetCulture(culture);
                Session["Culture"] = culture;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
        [System.Web.Http.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", Localization.CloudERP.Messages.Messages.EmailIsRequired);
                return View();
            }

            try
            {
                var success = await _httpClient.PostAsync("home/forgot-password", new { email });
                if (success) return View("ForgotPasswordEmailSent");

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: ResetPassword
        public async Task<ActionResult> ResetPassword(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync<dynamic>($"home/reset-password/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsAsync<dynamic>();
                    ViewBag.ResetCode = id;
                    return View();
                }

                return View("ResetPasswordLinkExpired");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: ResetPassword
        [System.Web.Http.HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string id, string newPassword, string confirmPassword)
        {
            try
            {
                var request = new ResetPasswordRequest
                {
                    ResetCode = id,
                    NewPassword = newPassword,
                    ConfirmPassword = confirmPassword
                };

                var success = await _httpClient.PostAsync("home/reset-password", request);
                if (success) return View("ResetPasswordSuccess");

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}