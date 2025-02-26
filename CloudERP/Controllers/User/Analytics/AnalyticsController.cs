using CloudERP.Helpers;
using Domain.Models;
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
                var model = await _httpClientHelper.GetAsync<AnalyticsModel>(
                    $"analyticsapi/getanalytics?companyID={_sessionHelper.CompanyID}");

                ViewBag.ChartData = new
                {
                    Employees = new { model?.TotalEmployees, model?.NewEmployeesThisMonth, model?.NewEmployeesThisYear },
                    Stock = new { model?.TotalStockItems, model?.StockAvailable, model?.StockExpired },
                    Support = new { model?.TotalSupportTickets, model?.ResolvedSupportTickets, model?.PendingSupportTickets }
                };

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