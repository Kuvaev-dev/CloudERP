using CloudERP.Helpers;
using CloudERP.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Company
{
    public class CompanyRegistrationController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public CompanyRegistrationController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: CompanyRegistration/RegistrationForm
        public ActionResult RegistrationForm()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new RegistrationMV());
        }

        // POST: CompanyRegistration/RegistrationForm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegistrationForm(RegistrationMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Please provide correct details";
                return View(model);
            }

            try
            {
                bool isSuccess = await _httpClient.PostAsync("companyregistrationapi/register", model);
                if (isSuccess)
                {
                    ViewBag.Message = "Registration successfull";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Unexpected error. Try again, please");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return View(model);
        }
    }
}
