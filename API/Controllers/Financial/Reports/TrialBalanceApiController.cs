using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/trial-balance")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TrialBalanceApiController : ApiController
    {
        private readonly ITrialBalanceRepository _trialBalanceRepository;
        private readonly IFinancialYearRepository _financialYearRepository;

        public TrialBalanceApiController(
            ITrialBalanceRepository trialBalanceRepository,
            IFinancialYearRepository financialYearRepository)
        {
            _trialBalanceRepository = trialBalanceRepository ?? throw new ArgumentNullException(nameof(ITrialBalanceRepository));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
        }

        // GET: BalanceSheet
        [HttpGet, Route("branch-trial-balance?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetBalanceSheet([FromUri] int companyId, [FromUri] int branchId)
        {
            try
            {
                var financialYear = await GetActiveAsync();

                var trialBalance = await _trialBalanceRepository.GetTrialBalanceAsync(
                    companyId,
                    branchId,
                    financialYear.FinancialYearID);

                return Ok(trialBalance);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("branch-trial-balance?companyId={companyId:int}&branchId={branchId:int}&financialYearId={financialYearId:int}")]
        public async Task<IHttpActionResult> GetBalanceSheet([FromUri] int companyId, [FromUri] int branchId, [FromUri] int financialYearId)
        {
            try
            {
                var balanceSheet = await _trialBalanceRepository.GetTrialBalanceAsync(
                    companyId,
                    branchId,
                    financialYearId);

                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("sub-branch-trial-balance?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetSubBalanceSheet([FromUri] int companyId, [FromUri] int branchId)
        {
            try
            {
                var financialYear = await GetActiveAsync();

                var balanceSheet = await _trialBalanceRepository.GetTrialBalanceAsync(
                    companyId,
                    branchId,
                    financialYear.FinancialYearID);

                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("branch-trial-balance?companyId={companyId:int}&branchId={branchId:int}&financialYearId={financialYearId:int}")]
        public async Task<IHttpActionResult> GetSubBalanceSheet([FromUri] int companyId, [FromUri] int branchId, [FromUri] int financialYearId)
        {
            try
            {
                await GetActiveAsync();

                var balanceSheet = await _trialBalanceRepository.GetTrialBalanceAsync(
                    companyId,
                    branchId,
                    financialYearId);

                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private async Task<FinancialYear> GetActiveAsync() => await _financialYearRepository.GetSingleActiveAsync();
    }
}