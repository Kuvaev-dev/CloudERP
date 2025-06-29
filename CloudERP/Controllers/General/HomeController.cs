﻿using Domain.Models.FinancialModels;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Localization;
using Domain.UtilsAccess;
using System.Text;

namespace CloudERP.Controllers.General
{
    public class HomeController : Controller
    {
        private readonly ISessionHelper _sessionHelper;
        private readonly IHttpClientHelper _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private const int ADMIN_USER_TYPE_ID = 1;

        public HomeController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IHttpContextAccessor httpContextAccessor)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public async Task<IActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            try
            {
                var dashboardValues = await _httpClient.GetAsync<DashboardModel>($"homeapi/getdashboardvalues?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(dashboardValues ?? new DashboardModel());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public IActionResult SetCurrency(string currency)
        {
            HttpContext.Session.SetString("SelectedCurrency", currency);
            return Redirect(HttpContext.Request.Headers.Referer.ToString());
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
                    ViewBag.ErrorMessage = Localization.CloudERP.Messages.Messages.PleaseProvideCorrectDetails;
                    return View("Login");
                }

                await SignInUser(userData.User, userData.Token);
                SetEmployeeSession(userData.Employee);
                SetCompanySession(userData.Company);

                if (userData.Employee.IsFirstLogin.HasValue && userData.Employee.IsFirstLogin.Value)
                    HttpContext.Session.SetString("StartTour", "true");

                var currencies = await _httpClient.GetAsync<Dictionary<string, decimal>>("homeapi/getcurrencies");
                ViewBag.Currencies = currencies ?? [];
                ViewBag.SelectedCurrency = HttpContext.Session.GetString("SelectedCurrency") ?? "UAH";

                return userData.User.UserTypeID == ADMIN_USER_TYPE_ID
                    ? RedirectToAction("Index", "UserType")
                    : RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Home");
        }

        [HttpPost]
        public IActionResult SetCulture(string culture, string returnUrl = "/Guide/PrivacyPolicy")
        {
            var supportedCultures = new List<string> { "en-US", "uk-UA" };

            if (string.IsNullOrEmpty(culture) || !supportedCultures.Contains(culture))
            {
                culture = "en-US";
            }

            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                }
            );

            return LocalRedirect(Url.IsLocalUrl(returnUrl) ? returnUrl : "/Guide/PrivacyPolicy");
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
                ModelState.AddModelError("", Localization.CloudERP.Messages.Messages.EmailIsRequired);
                return View();
            }

            try
            {
                var success = await _httpClient.PostAsync("homeapi/forgotpassword", new ForgotPasswordRequest { Email = email });
                if (success) return View("ForgotPasswordEmailSent");

                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<IActionResult> ResetPassword(string id)
        {
            try
            {
                var response = await _httpClient.GetAsync<object>($"homeapi/getresetpassword?id={id}");
                if (response != null)
                {
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

                var success = await _httpClient.PostAsync("home/resetpassword", request);
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