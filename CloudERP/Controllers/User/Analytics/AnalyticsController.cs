using CloudERP.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.User.Analytics
{
    public class AnalyticsController : Controller
    {
        private readonly HttpClientHelper _httpClientHelper;
        private readonly SessionHelper _sessionHelper;

        public AnalyticsController(
            HttpClientHelper httpClientHelper,
            SessionHelper sessionHelper)
        {
            _httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var analyticsData = await _httpClientHelper.GetAsync<dynamic>("analytics/index");

                var model = analyticsData?.model;
                var chartData = analyticsData?.chartData;

                ViewBag.ChartData = chartData;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}