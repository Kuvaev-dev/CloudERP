using CloudERP.Helpers;
using Utils.Helpers;
using Domain.Models.FinancialModels;
using Domain.Models;
using CloudERP.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace CloudERP.Controllers.General
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

        public async Task<IActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            try
            {
                var dashboardValues = await _httpClient.GetAsync<DashboardModel>($"home/dashboard/{_sessionHelper.BranchID}/{_sessionHelper.CompanyID}");
                var currencies = await _httpClient.GetAsync<Dictionary<string, decimal>>("home/currencies");

                ViewBag.Currencies = currencies;
                ViewBag.SelectedCurrency = HttpContext.Session.GetString("SelectedCurrency") ?? "UAH";

                return View(dashboardValues);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public IActionResult Login()
        {
            ViewBag.RememberedEmail = Request.Cookies["RememberMe"] == null ? Request.Cookies["Email"] : string.Empty;
            return View(new LoginRequest() { RememberMe = false });
        }

        [HttpPost]
        public async Task<IActionResult> LoginUser([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var user = await _httpClient.PostAndReturnAsync<Domain.Models.User>("home/login", loginRequest);
                if (user == null)
                {
                    ViewBag.Message = Localization.CloudERP.Messages.Messages.PleaseProvideCorrectDetails;
                    return View("Login");
                }

                await SignInUser(user);

                var employee = await _httpClient.PostAndReturnAsync<Employee>("home/employee", new { user.UserID });
                if (employee == null)
                {
                    ClearSession();
                    return RedirectToAction("Login", "Home");
                }

                SetEmployeeSession(employee);

                var company = await _httpClient.PostAndReturnAsync<Domain.Models.Company>("home/company", new { employee.CompanyID });
                if (company == null)
                {
                    ClearSession();
                    return RedirectToAction("Login", "Home");
                }

                SetCompanySession(company);

                var isFirstLogin = await _httpClient.PostAsync<bool>("home/isFirstLogin", new { employee.UserID });
                if (isFirstLogin) HttpContext.Session.SetString("StartTour", "true");

                var currencies = await _httpClient.GetAsync<Dictionary<string, decimal>>("home/currencies");
                ViewBag.Currencies = currencies;

                return user.UserTypeID == ADMIN_USER_TYPE_ID ? RedirectToAction("AdminMenuGuide", "Guide") : RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return View("Login");
            }
        }

        private async Task SignInUser(Domain.Models.User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Email),
                new("UserID", user.UserID.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Session.SetString("UserID", user.UserID.ToString());
            HttpContext.Session.SetString("UserTypeID", user.UserTypeID.ToString());
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetString("Email", user.Email);
            HttpContext.Session.SetString("ContactNo", user.ContactNo);
            HttpContext.Session.SetString("UserName", user.UserName);
            HttpContext.Session.SetString("Password", user.Password);
            HttpContext.Session.SetString("Salt", user.Salt);
            HttpContext.Session.SetString("IsActive", user.IsActive.ToString());
        }

        private void SetEmployeeSession(Employee employee)
        {
            HttpContext.Session.SetString("EName", employee.FullName);
            HttpContext.Session.SetString("EPhoto", employee.Photo);
            HttpContext.Session.SetString("ERegistrationDate", employee.RegistrationDate.ToString() ?? "");
            HttpContext.Session.SetString("Designation", employee.Designation);
            HttpContext.Session.SetString("BranchID", employee.BranchID.ToString());
            HttpContext.Session.SetString("BranchTypeID", employee.BranchTypeID.ToString());
            HttpContext.Session.SetString("BrchID", employee.BrchID.ToString() ?? "");
            HttpContext.Session.SetString("CompanyID", employee.CompanyID.ToString());
        }

        private void SetCompanySession(Domain.Models.Company company)
        {
            HttpContext.Session.SetString("CName", company.Name);
            HttpContext.Session.SetString("CLogo", company.Logo);
        }

        private void ClearSession()
        {
            HttpContext.Session.Clear();
        }

        public IActionResult Logout()
        {
            ClearSession();
            return RedirectToAction("Login");
        }

        public IActionResult SetCulture(string culture)
        {
            try
            {
                _resourceManagerHelper.SetCulture(culture);
                HttpContext.Session.SetString("Culture", culture);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return Redirect(Request.Headers["Referer"].ToString());
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
                var success = await _httpClient.PostAsync<string>("home/forgot-password", new { email });
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
                var response = await _httpClient.GetAsync<dynamic>($"home/reset-password/{id}");
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

                var success = await _httpClient.PostAsync<ResetPasswordRequest>("home/reset-password", request);
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