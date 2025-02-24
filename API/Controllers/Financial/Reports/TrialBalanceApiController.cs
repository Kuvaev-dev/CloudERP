using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Financial.Reports
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class TrialBalanceApiController : ControllerBase
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
        [HttpGet]
        public async Task<ActionResult<List<TrialBalanceModel>>> GetTrialBalance(int companyId, int branchId)
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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<TrialBalanceModel>>> GetTrialBalanceByFinancialYear(int companyId, int branchId, int financialYearId)
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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private async Task<FinancialYear> GetActiveAsync() => await _financialYearRepository.GetSingleActiveAsync();
    }
}