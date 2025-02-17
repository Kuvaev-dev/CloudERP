using CloudERP.Helpers;
using CloudERP.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyRegistrationController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public CompanyRegistrationController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
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
                ViewBag.Message = "Пожалуйста, введите корректные данные.";
                return View(model);
            }

            try
            {
                bool isSuccess = await _httpClient.PostAsync("company-registration/register", model);
                if (isSuccess)
                {
                    ViewBag.Message = "Регистрация прошла успешно.";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Ошибка при регистрации. Попробуйте снова.");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла ошибка: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return View(model);
        }
    }
}
