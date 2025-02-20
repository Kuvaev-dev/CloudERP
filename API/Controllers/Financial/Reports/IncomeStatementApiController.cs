using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Financial.Reports
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class IncomeStatementApiController : ControllerBase
    {
        private readonly IIncomeStatementService _incomeStatementService;
        private readonly IFinancialYearRepository _financialYearRepository;

        public IncomeStatementApiController(
            IIncomeStatementService incomeStatementService,
            IFinancialYearRepository financialYearRepository)
        {
            _incomeStatementService = incomeStatementService ?? throw new ArgumentNullException(nameof(IIncomeStatementService));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
        }

        // GET: IncomeStatement
        [HttpGet]
        public async Task<ActionResult<IncomeStatementModel>> GetIncomeStatement(int companyId, int branchId)
        {
            try
            {
                var FinancialYear = await GetActiveAsync();

                return Ok(await _incomeStatementService.GetIncomeStatementAsync(companyId, branchId, FinancialYear.FinancialYearID));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IncomeStatementModel>> GetIncomeStatementByFinancialYear(int companyId, int branchId, int FinancialYearID)
        {
            try
            {
                return Ok(await _incomeStatementService.GetIncomeStatementAsync(companyId, branchId, FinancialYearID));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: IncomeStatement
        [HttpGet]
        public async Task<ActionResult<IncomeStatementModel>> GetSubIncomeStatement(int companyId, int branchId)
        {
            try
            {
                var FinancialYear = await GetActiveAsync();

                return Ok(await _incomeStatementService.GetIncomeStatementAsync(companyId, branchId, FinancialYear.FinancialYearID));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IncomeStatementModel>> GetSubIncomeStatementByFinancialYear(int companyId, int branchId, int FinancialYearID)
        {
            try
            {
                var FinancialYear = await GetActiveAsync();

                return Ok(await _incomeStatementService.GetIncomeStatementAsync(companyId, branchId, FinancialYearID));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private async Task<FinancialYear> GetActiveAsync() => await _financialYearRepository.GetSingleActiveAsync();
    }
}