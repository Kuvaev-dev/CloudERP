using CloudERP.Helpers;
using Domain.Models;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class EmployeeStatisticsController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly IEmployeeStatisticsService _employeeStatsService;

        public EmployeeStatisticsController(
            SessionHelper sessionHelper,
            IEmployeeStatisticsService employeeStatsService)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _employeeStatsService = employeeStatsService ?? throw new ArgumentNullException(nameof(employeeStatsService));
        }

        // GET: EmployeeStatistics
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var statistics = await GetStatisticsAsync(DateTime.Now.AddMonths(-1), DateTime.Now);
                return View(statistics);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                return View(statistics);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task<List<EmployeeStatistics>> GetStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            DateTime start = startDate ?? DateTime.Now.AddMonths(-1);
            DateTime end = endDate ?? DateTime.Now;

            var statistics = await _employeeStatsService.GetStatisticsAsync(
                start,
                end,
                _sessionHelper.BranchID,
                _sessionHelper.CompanyID
            );

            var chartData = new
            {
                Labels = statistics.Select(s => s.Date.ToString("yyyy-MM-dd")).ToList(),
                Data = statistics.Select(s => s.NumberOfRegistrations).ToList()
            };

            ViewBag.ChartData = chartData;
            return statistics.ToList();
        }
    }
}