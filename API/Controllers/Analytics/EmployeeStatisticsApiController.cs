using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Analytics
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class EmployeeStatisticsApiController : ControllerBase
    {
        private readonly IEmployeeStatisticsService _employeeStatsService;

        public EmployeeStatisticsApiController(IEmployeeStatisticsService employeeStatsService)
        {
            _employeeStatsService = employeeStatsService ?? throw new ArgumentNullException(nameof(employeeStatsService));
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetEmployeeStatistics(DateTime? startDate = null, DateTime? endDate = null, int companyId = 0, int branchId = 0)
        {
            try
            {
                DateTime start = startDate ?? DateTime.Now.AddMonths(-1);
                DateTime end = endDate ?? DateTime.Now;

                var statistics = await _employeeStatsService.GetStatisticsAsync(
                    start,
                    end,
                    branchId,
                    companyId
                );

                var chartData = new
                {
                    Labels = statistics.Select(s => s.Date.ToString("yyyy-MM-dd")).ToList(),
                    Data = statistics.Select(s => s.NumberOfRegistrations).ToList()
                };

                return Ok(new { statistics, chartData });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}