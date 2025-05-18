using CloudERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CloudERP.Controllers.Utilities.Support
{
    public class GuideController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        public GuideController(SessionHelper sessionHelper, HttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Guide
        public async Task<ActionResult> AdminMenuGuide()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            var currencies = await _httpClient.GetAsync<Dictionary<string, string>>("homeapi/getcurrencies");

            ViewBag.Currencies = currencies?.ToDictionary(
                k => k.Key,
                v => decimal.TryParse(v.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed) ? parsed : 0m
            );

            return View();
        }

        public ActionResult MainBranchEmployeeGuide()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult EmployeeGuide()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login");

            return View();
        }
    }
}