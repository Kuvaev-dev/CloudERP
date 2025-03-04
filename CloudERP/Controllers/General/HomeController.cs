using CloudERP.Helpers;
using Domain.Models.FinancialModels;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;

namespace CloudERP.Controllers.General
{
    public class HomeController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        private const int ADMIN_USER_TYPE_ID = 1;

        public HomeController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            try
            {
                var dashboardValues = await _httpClient.GetAsync<DashboardModel>($"homeapi/getdashboardvalues?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                var currencies = await _httpClient.GetAsync<Dictionary<string, decimal>>("homeapi/getcurrencies");

                ViewBag.Currencies = currencies;
                ViewBag.SelectedCurrency = HttpContext.Session.GetString("SelectedCurrency") ?? "UAH";

                return View(dashboardValues ?? new DashboardModel());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public IActionResult Login()
        {
            ViewBag.RememberedEmail = Request.Cookies["RememberMe"] == null ? Request.Cookies["Email"] : string.Empty;
            return View(new LoginRequest() { RememberMe = false });
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser(LoginRequest loginRequest)
        {
            try
            {
                var userData = await _httpClient.PostAndReturnAsync<LoginResponse>("homeapi/loginuser", loginRequest);

                if (userData?.User == null)
                {
                    TempData["ErrorMessage"] = Localization.CloudERP.Messages.PleaseProvideCorrectDetails;
                    return View("Login");
                }

                await SignInUser(userData.User, userData.Token);
                SetEmployeeSession(userData.Employee);
                SetCompanySession(userData.Company);

                if (userData.Employee.IsFirstLogin.HasValue && userData.Employee.IsFirstLogin.Value)
                    HttpContext.Session.SetString("StartTour", "true");

                var currencies = await _httpClient.GetAsync<Dictionary<string, string>>("homeapi/getcurrencies");
                ViewBag.Currencies = currencies ?? new Dictionary<string, string>();

                return userData.User.UserTypeID == ADMIN_USER_TYPE_ID
                    ? View("~/Views/Guide/AdminMenuGuide.cshtml")
                    : View("~/Views/Guide/PrivacyPolicy.cshtml");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return View("Login");
            }
        }

        private async Task SignInUser(Domain.Models.User user, string token)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email),
                new("UserID", user.UserID.ToString()),
                new("token", token)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetInt32("UserID", user.UserID);
            HttpContext.Session.SetInt32("UserTypeID", user.UserTypeID);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("ContactNo", user.ContactNo);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Password", user.Password);
            HttpContext.Session.SetString("Salt", user.Salt);
            HttpContext.Session.SetString("IsActive", user.IsActive.ToString());
            HttpContext.Session.SetString("Token", token);
            HttpContext.Session.SetString("Culture", "en-US");
        }

        private void SetEmployeeSession(Employee employee)
        {
            HttpContext.Session.SetString("EName", employee.FullName);
            HttpContext.Session.SetString("EPhoto", employee.Photo);
            HttpContext.Session.SetString("ERegistrationDate", employee.RegistrationDate.ToString() ?? "");
            HttpContext.Session.SetString("Designation", employee.Designation);
            HttpContext.Session.SetInt32("BranchID", employee.BranchID);
            HttpContext.Session.SetInt32("BranchTypeID", employee.BranchTypeID);
            HttpContext.Session.SetInt32("BrchID", employee.BrchID ?? 0);
            HttpContext.Session.SetInt32("CompanyID", employee.CompanyID);
        }

        private void SetCompanySession(Domain.Models.Company company)
        {
            HttpContext.Session.SetString("CName", company.Name);
            HttpContext.Session.SetString("CLogo", company.Logo);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult SetCulture(string culture, string returnUrl = "/")
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", Localization.CloudERP.Messages.EmailIsRequired);
                return View();
            }

            try
            {
                var success = await _httpClient.PostAsync("homeapi/forgotpassword", new { email });
                if (success) return View("ForgotPasswordEmailSent");

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<IActionResult> ResetPassword(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync<dynamic>($"homeapi/getresetpassword?id={id}");
                if (response?.IsSuccessStatusCode)
                {
                    var content = await response?.Content.ReadAsAsync<dynamic>();
                    ViewBag.ResetCode = id;
                    return View();
                }

                return View("ResetPasswordLinkExpired");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id, string newPassword, string confirmPassword)
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
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}