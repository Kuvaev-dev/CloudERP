using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/employee-statistics")]
    public class EmployeeStatisticsApiController : ApiController
    {
        private readonly IEmployeeStatisticsService _employeeStatsService;

        public EmployeeStatisticsApiController(
            IEmployeeStatisticsService employeeStatsService)
        {
            _employeeStatsService = employeeStatsService ?? throw new ArgumentNullException(nameof(employeeStatsService));
        }

        [HttpGet]
        [Route("statistics")]
        public async Task<IHttpActionResult> GetEmployeeStatistics(DateTime? startDate = null, DateTime? endDate = null, int companyId = 0, int branchId = 0)
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
                return InternalServerError(ex);
            }
        }
    }
}