using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/income-statement")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class IncomeStatementApiController : ApiController
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
        [HttpGet, Route("branch-income-statement/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetIncomeStatement(int companyId, int branchId)
        {
            try
            {
                var FinancialYear = await GetActiveAsync();

                return Ok(await _incomeStatementService.GetIncomeStatementAsync(companyId, branchId, FinancialYear.FinancialYearID));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("branch-income-statement/{companyId:int}/{branchId:int}/{FinancialYearID:int}")]
        public async Task<IHttpActionResult> GetIncomeStatementByFinancialYear(int companyId, int branchId, int FinancialYearID)
        {
            try
            {
                return Ok(await _incomeStatementService.GetIncomeStatementAsync(companyId, branchId, FinancialYearID));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: IncomeStatement
        [HttpGet, Route("sub-branch-income-statement/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetSubIncomeStatement(int companyId, int branchId)
        {
            try
            {
                var FinancialYear = await GetActiveAsync();

                return Ok(await _incomeStatementService.GetIncomeStatementAsync(companyId, branchId, FinancialYear.FinancialYearID));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("sub-branch-income-statement/{companyId:int}/{branchId:int}/{FinancialYearID:int}")]
        public async Task<IHttpActionResult> GetSubIncomeStatementByFinancialYear(int companyId, int branchId, int FinancialYearID)
        {
            try
            {
                var FinancialYear = await GetActiveAsync();

                return Ok(await _incomeStatementService.GetIncomeStatementAsync(companyId, branchId, FinancialYearID));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private async Task<FinancialYear> GetActiveAsync() => await _financialYearRepository.GetSingleActiveAsync();
    }
}