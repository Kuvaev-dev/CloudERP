using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Analytics
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
                    Employees = new
                    {
                        Total = model?.TotalEmployees ?? 0,
                        NewInAMonth = model?.NewEmployeesThisMonth ?? 0,
                        NewInAYear = model?.NewEmployeesThisYear ?? 0
                    },
                    Stock = new
                    {
                        TotalItems = model?.TotalStockItems ?? 0,
                        Available = model?.StockAvailable ?? 0,
                        Expired = model?.StockExpired ?? 0
                    },
                    Support = new
                    {
                        TotalTickets = model?.TotalSupportTickets ?? 0,
                        Resolved = model?.ResolvedSupportTickets ?? 0,
                        Pending = model?.PendingSupportTickets ?? 0
                    }
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