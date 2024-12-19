using CloudERP.Helpers;
using Domain.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class EmployeeStatisticsController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly IEmployeeStatisticsService _employeeStatsService;

        public EmployeeStatisticsController(SessionHelper sessionHelper, IEmployeeStatisticsService employeeStatsService)
        {
            _sessionHelper = sessionHelper;
            _employeeStatsService = employeeStatsService;
        }

        // GET: EmployeeStatistics
        public async Task<ActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                DateTime start = startDate ?? DateTime.Now.AddMonths(-1);
                DateTime end = endDate ?? DateTime.Now;

                return View(await _employeeStatsService.GetStatisticsAsync(start, end, _sessionHelper.BranchID, _sessionHelper.CompanyID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}