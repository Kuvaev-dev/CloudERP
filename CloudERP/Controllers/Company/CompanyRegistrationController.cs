using CloudERP.Models;
using Domain.UtilsAccess;
using Localization.CloudERP.Messages;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Company
{
    public class CompanyRegistrationController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public CompanyRegistrationController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
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
                ViewBag.Message = Messages.PleaseProvideCorrectDetails;
                return View(model);
            }

            try
            {
                bool isSuccess = await _httpClient.PostAsync("companyregistrationapi/register", model);
                if (isSuccess) return RedirectToAction("Login", "Home");
                else ModelState.AddModelError("", Messages.UnexpectedErrorMessage);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("Error", "Home");
            }

            return View(model);
        }
    }
}
