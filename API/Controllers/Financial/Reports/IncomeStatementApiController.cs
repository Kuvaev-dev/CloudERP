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
        [HttpGet, Route("branch-income-statement?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetIncomeStatement([FromUri] int companyId, [FromUri] int branchId)
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

        [HttpGet, Route("branch-income-statement?companyId={companyId:int}&branchId={branchId:int}&FinancialYearID={FinancialYearID:int}")]
        public async Task<IHttpActionResult> GetIncomeStatementByFinancialYear([FromUri] int companyId, [FromUri] int branchId, [FromUri] int FinancialYearID)
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
        [HttpGet, Route("sub-branch-income-statement?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetSubIncomeStatement([FromUri] int companyId, [FromUri] int branchId)
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

        [HttpGet, Route("sub-branch-income-statement?companyId={companyId:int}&branchId={branchId:int}&FinancialYearID={FinancialYearID:int}")]
        public async Task<IHttpActionResult> GetSubIncomeStatementByFinancialYear([FromUri] int companyId, [FromUri] int branchId, [FromUri] int FinancialYearID)
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