using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Financial.Reports
{
    [ApiController]
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
        public async Task<ActionResult<List<TrialBalanceModel>>> GetBalanceSheet(int companyId, int branchId)
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
        public async Task<ActionResult<List<TrialBalanceModel>>> GetBalanceSheetByFinancialYear(int companyId, int branchId, int financialYearId)
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

        [HttpGet]
        public async Task<ActionResult<List<TrialBalanceModel>>> GetSubBalanceSheet(int companyId, int branchId)
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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<TrialBalanceModel>>> GetSubBalanceSheetByFinancialYear(int companyId, int branchId, int financialYearId)
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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private async Task<FinancialYear> GetActiveAsync() => await _financialYearRepository.GetSingleActiveAsync();
    }
}