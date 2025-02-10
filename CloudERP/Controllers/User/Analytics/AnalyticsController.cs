using Domain.RepositoryAccess;
using Domain.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;

namespace CloudERP.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISupportTicketRepository _supportTicketRepository;
        private readonly SessionHelper _sessionHelper;

        public AnalyticsController(
            IEmployeeRepository employeeRepository,
            IStockRepository stockRepository,
            ISupportTicketRepository supportTicketRepository,
            SessionHelper sessionHelper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(IStockRepository));
            _supportTicketRepository = supportTicketRepository ?? throw new ArgumentNullException(nameof(ISupportTicketRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var model = new AnalyticsModel
                {
                    // Users
                    TotalEmployees = await _employeeRepository.GetCountByCompanyAsync(_sessionHelper.CompanyID),
                    NewEmployeesThisMonth = await _employeeRepository.GetMonthNewEmployeesCountByCompanyAsync(_sessionHelper.CompanyID),
                    NewEmployeesThisYear = await _employeeRepository.GetYearNewEmployeesCountByCompanyAsync(_sessionHelper.CompanyID),

                    // Stock
                    TotalStockItems = await _stockRepository.GetTotalStockItemsByCompanyAsync(_sessionHelper.CompanyID),
                    StockAvailable = await _stockRepository.GetTotalAvaliableItemsByCompanyAsync(_sessionHelper.CompanyID),
                    StockExpired = await _stockRepository.GetTotalExpiredItemsByCompanyAsync(_sessionHelper.CompanyID),

                    // Support
                    TotalSupportTickets = await _supportTicketRepository.GetTotalSupportTicketsByCompany(_sessionHelper.CompanyID),
                    ResolvedSupportTickets = await _supportTicketRepository.GetTotalResolvedSupportTicketsByCompany(_sessionHelper.CompanyID),
                    PendingSupportTickets = await _supportTicketRepository.GetTotalPendingSupportTicketsByCompany(_sessionHelper.CompanyID)
                };

                ViewBag.ChartData = new
                {
                    Employees = new { Total = model.TotalEmployees, NewInAMonth = model.NewEmployeesThisMonth, NewInAYear = model.NewEmployeesThisYear },
                    Stock = new { TotalItems = model.TotalStockItems, Available = model.StockAvailable, Expired = model.StockExpired },
                    Support = new { TotalTickets = model.TotalSupportTickets, Resolved = model.ResolvedSupportTickets, Pending = model.PendingSupportTickets }
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}