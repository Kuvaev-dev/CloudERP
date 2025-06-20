using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Analytics
{
    public class EmployeeStatisticsController : Controller
    {
        private readonly IHttpClientHelper _httpClientHelper;
        private readonly ISessionHelper _sessionHelper;

        public EmployeeStatisticsController(
            IHttpClientHelper httpClientHelper,
            ISessionHelper sessionHelper)
        {
            _httpClientHelper = httpClientHelper ?? throw new ArgumentNullException(nameof(httpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
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

            var statistics = await _httpClientHelper.GetAsync<IEnumerable<EmployeeStatistics>>(
                $"employeestatisticsapi/getemployeestatistics" +
                $"?startDate={start:yyyy-MM-dd}&endDate={end:yyyy-MM-dd}" +
                $"&companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}"
            );

            var chartData = new
            {
                Labels = statistics?.Select(s => s.Date.ToString("yyyy-MM-dd")).ToList(),
                Data = statistics?.Select(s => s.NumberOfRegistrations).ToList()
            };

            ViewBag.ChartData = chartData;

            return statistics?.ToList() ?? new List<EmployeeStatistics>();
        }
    }
}