using CloudERP.Helpers;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class EmployeeStatisticsController : Controller
    {
        private readonly HttpClientHelper _httpClientHelper;
        private readonly SessionHelper _sessionHelper;

        public EmployeeStatisticsController(HttpClientHelper httpClientHelper, SessionHelper sessionHelper)
        {
            _httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(httpClientHelper));
            _sessionHelper = sessionHelper;
        }

        // GET: EmployeeStatistics
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var statistics = await GetStatisticsAsync(DateTime.Now.AddMonths(-1), DateTime.Now);
                if (statistics == null) return RedirectToAction("EP404", "EP");

                return View(statistics);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: EmployeeStatistics
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var statistics = await GetStatisticsAsync(startDate, endDate);
                if (statistics == null) return RedirectToAction("EP404", "EP");

                return View(statistics);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task<List<EmployeeStatistics>> GetStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Now.AddMonths(-1);
            DateTime end = endDate ?? DateTime.Now;

            var statistics = await _httpClientHelper.GetAsync<List<EmployeeStatistics>>(
                $"api/employee-statistics/statistics?startDate={start:yyyy-MM-dd}&endDate={end:yyyy-MM-dd}&companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}"
            );

            var chartData = new
            {
                Labels = statistics.Select(s => s.Date.ToString("yyyy-MM-dd")).ToList(),
                Data = statistics.Select(s => s.NumberOfRegistrations).ToList()
            };

            ViewBag.ChartData = chartData;
            return statistics;
        }
    }
}