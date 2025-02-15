using CloudERP.Helpers;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Threading.Tasks;
using System.Collections.Generic;
using Services.Facades;
using Utils.Helpers;

namespace CloudERP.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeFacade _homeFacade;
        private readonly SessionHelper _sessionHelper;
        private readonly ResourceManagerHelper _resourceManagerHelper;
        private readonly HttpClientHelper _httpClient;

        private const int MAIN_BRANCH_TYPE_ID = 1;

        public HomeController(
            HomeFacade homeFacade, 
            SessionHelper sessionHelper, 
            ResourceManagerHelper resourceManagerHelper,
            HttpClientHelper httpClient)
        {
            _homeFacade = homeFacade ?? throw new ArgumentNullException(nameof(HomeFacade));
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
                var dashboardValues = await _httpClient.GetAsync<object>($"home/dashboard?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}");
                var currencies = await _httpClient.GetAsync<List<object>>("home/currencies");

                ViewBag.Currencies = currencies;
                ViewBag.SelectedCurrency = Session["SelectedCurrency"] as string ?? "UAH";

                return View(await _homeFacade.DashboardService.GetDashboardValues(_sessionHelper.BranchID, _sessionHelper.CompanyID));
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
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> LoginUser(string email, string password, bool? rememberMe)
        {
            try
            {
                var user = await _homeFacade.AuthService.AuthenticateUserAsync(email, password);
                if (user != null)
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

                    var employee = await _homeFacade.EmployeeRepository.GetByUserIdAsync(user.UserID);
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

                    var company = await _homeFacade.CompanyRepository.GetByIdAsync(employee.CompanyID);
                    if (company == null)
                    {
                        ClearSession();
                        return RedirectToAction("Login", "Home");
                    }

                    Session["CName"] = company.Name;
                    Session["CLogo"] = company.Logo;

                    if (await _homeFacade.EmployeeRepository.IsFirstLoginAsync(employee))
                    {
                        Session["StartTour"] = true;
                    }

                    var currencies = await _httpClient.GetAsync<List<object>>("home/currencies");
                    ViewBag.Currencies = currencies;

                    return user.UserTypeID == MAIN_BRANCH_TYPE_ID ? RedirectToAction("AdminMenuGuide", "Guide") : RedirectToAction("Index", "Home");
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
        [HttpPost]
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
                if (await _homeFacade.AuthService.IsPasswordResetRequestedRecentlyAsync(email))
                {
                    ModelState.AddModelError("", Localization.CloudERP.Messages.Messages.PasswordResetAlreadyRequested);
                    return View();
                }

                var user = await _homeFacade.UserRepository.GetByEmailAsync(email);
                if (user != null)
                {
                    user.ResetPasswordCode = Guid.NewGuid().ToString();
                    user.ResetPasswordExpiration = DateTime.Now.AddHours(1);
                    user.LastPasswordResetRequest = DateTime.Now;

                    await _homeFacade.UserRepository.UpdateAsync(user);

                    var resetLink = Url.Action("ResetPassword", "Home", new { id = user.ResetPasswordCode }, protocol: Request.Url.Scheme);
                    _homeFacade.AuthService.SendPasswordResetEmailAsync(resetLink, user.Email, user.ResetPasswordCode);

                    return View("ForgotPasswordEmailSent");
                }

                return View("ForgotPasswordEmailSent");
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
                var user = await _homeFacade.UserRepository.GetByPasswordCodesAsync(id, DateTime.Now);
                if (user != null)
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

        // POST: ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(string id, string newPassword, string confirmPassword)
        {
            try
            {
                if (!await _homeFacade.AuthService.ResetPasswordAsync(id, newPassword, confirmPassword))
                {
                    ModelState.AddModelError("", "Passwords do not match or link expired.");
                    return View();
                }

                return View("ResetPasswordSuccess");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}