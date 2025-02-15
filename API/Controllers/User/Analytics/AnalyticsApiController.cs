using System;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/analytics")]
    public class AnalyticsApiController : ApiController
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISupportTicketRepository _supportTicketRepository;

        public AnalyticsApiController(
            IEmployeeRepository employeeRepository,
            IStockRepository stockRepository,
            ISupportTicketRepository supportTicketRepository)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(IStockRepository));
            _supportTicketRepository = supportTicketRepository ?? throw new ArgumentNullException(nameof(ISupportTicketRepository));
        }

        [HttpGet]
        [Route("index")]
        public async Task<IHttpActionResult> GetAnalytics([FromUri] int companyID)
        {
            try
            {
                var model = new AnalyticsModel
                {
                    // Users
                    TotalEmployees = await _employeeRepository.GetCountByCompanyAsync(companyID),
                    NewEmployeesThisMonth = await _employeeRepository.GetMonthNewEmployeesCountByCompanyAsync(companyID),
                    NewEmployeesThisYear = await _employeeRepository.GetYearNewEmployeesCountByCompanyAsync(companyID),

                    // Stock
                    TotalStockItems = await _stockRepository.GetTotalStockItemsByCompanyAsync(companyID),
                    StockAvailable = await _stockRepository.GetTotalAvaliableItemsByCompanyAsync(companyID),
                    StockExpired = await _stockRepository.GetTotalExpiredItemsByCompanyAsync(companyID),

                    // Support
                    TotalSupportTickets = await _supportTicketRepository.GetTotalSupportTicketsByCompany(companyID),
                    ResolvedSupportTickets = await _supportTicketRepository.GetTotalResolvedSupportTicketsByCompany(companyID),
                    PendingSupportTickets = await _supportTicketRepository.GetTotalPendingSupportTicketsByCompany(companyID)
                };

                var chartData = new
                {
                    Employees = new { Total = model.TotalEmployees, NewInAMonth = model.NewEmployeesThisMonth, NewInAYear = model.NewEmployeesThisYear },
                    Stock = new { TotalItems = model.TotalStockItems, Available = model.StockAvailable, Expired = model.StockExpired },
                    Support = new { TotalTickets = model.TotalSupportTickets, Resolved = model.ResolvedSupportTickets, Pending = model.PendingSupportTickets }
                };

                return Ok(new { model, chartData });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}