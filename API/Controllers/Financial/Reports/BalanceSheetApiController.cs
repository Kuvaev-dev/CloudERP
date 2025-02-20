using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Financial.Reports
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BalanceSheetApiController : ControllerBase
    {
        private readonly IBalanceSheetService _balanceSheetService;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IAccountHeadRepository _accountHeadRepository;

        public BalanceSheetApiController(
            IBalanceSheetService balanceSheetService,
            IFinancialYearRepository financialYearRepository,
            IAccountHeadRepository accountHeadRepository)
        {
            _balanceSheetService = balanceSheetService ?? throw new ArgumentNullException(nameof(IBalanceSheetService));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(IAccountHeadRepository));
        }

        // GET: BalanceSheet
        [HttpGet]
        public async Task<ActionResult<BalanceSheetModel>> GetBalanceSheet(int companyId, int branchId)
        {
            try
            {
                var financialYear = await GetActiveAsync();

                var balanceSheet = await _balanceSheetService.GetBalanceSheetAsync(
                    companyId,
                    branchId,
                    financialYear.FinancialYearID,
                    await GetHeadsIDsList());

                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<BalanceSheetModel>> GetBalanceSheetByFinancialYear(int companyId, int branchId, int financialYearId)
        {
            try
            {
                var balanceSheet = await _balanceSheetService.GetBalanceSheetAsync(
                    companyId,
                    branchId,
                    financialYearId,
                    await GetHeadsIDsList());

                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet, Route("sub-branch-balance-sheet/{companyId:int}/{branchId:int}")]
        public async Task<ActionResult<BalanceSheetModel>> GetSubBalanceSheet(int companyId, int branchId)
        {
            try
            {
                var financialYear = await GetActiveAsync();

                var balanceSheet = await _balanceSheetService.GetBalanceSheetAsync(
                    companyId,
                    branchId,
                    financialYear.FinancialYearID,
                    await GetHeadsIDsList());

                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet, Route("branch-balance-sheet/{companyId:int}/{branchId:int}/{financialYearId:int}")]
        public async Task<ActionResult<BalanceSheetModel>> GetSubBalanceSheetByFinancialYear(int companyId, int branchId, int financialYearId)
        {
            try
            {
                await GetActiveAsync();

                var balanceSheet = await _balanceSheetService.GetBalanceSheetAsync(
                    companyId,
                    branchId,
                    financialYearId,
                    await GetHeadsIDsList());

                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private async Task<List<int>> GetHeadsIDsList()
        {
            IEnumerable<int> accountHeadsIDs = await _accountHeadRepository.GetAllIdsAsync();
            return accountHeadsIDs.ToList();
        }

        private async Task<FinancialYear> GetActiveAsync() => await _financialYearRepository.GetSingleActiveAsync();
    }
}