using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Financial.Reports
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class LedgerApiController : ControllerBase
    {
        private readonly ILedgerRepository _ledgerRepository;
        private readonly IFinancialYearRepository _financialYearRepository;

        public LedgerApiController(
            ILedgerRepository ledgerRepository,
            IFinancialYearRepository financialYearRepository)
        {
            _ledgerRepository = ledgerRepository ?? throw new ArgumentNullException(nameof(ILedgerRepository));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
        }

        [HttpGet]
        public async Task<ActionResult<List<AccountLedgerModel>>> GetLedger(int companyId, int branchId)
        {
            try
            {
                await PopulateViewBag();

                var defaultFinancialYear = await _financialYearRepository.GetSingleActiveAsync();
                if (defaultFinancialYear != null)
                {
                    return Ok(await _ledgerRepository.GetLedgerAsync(companyId, branchId, defaultFinancialYear.FinancialYearID));
                }

                return Ok(new List<AccountLedgerModel>());
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet, Route("branch-ledger/{companyId:int}/{branchId:int}/{financialYearId:int}")]
        public async Task<ActionResult<List<AccountLedgerModel>>> GetLedgerByFinancialYear(int companyId, int branchId, int financialYearId)
        {
            try
            {
                await PopulateViewBag(financialYearId);

                return Ok(await _ledgerRepository.GetLedgerAsync(companyId, branchId, financialYearId));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet, Route("sub-branch-ledger/{companyId:int}/{branchId:int}")]
        public async Task<ActionResult<List<AccountLedgerModel>>> GetSubLedger(int companyId, int branchId)
        {
            try
            {
                await PopulateViewBag();

                var defaultFinancialYear = await _financialYearRepository.GetSingleActiveAsync();
                if (defaultFinancialYear != null)
                {
                    return Ok(await _ledgerRepository.GetLedgerAsync(companyId, branchId, defaultFinancialYear.FinancialYearID));
                }

                return Ok(new List<AccountLedgerModel>());
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet, Route("sub-branch-ledger/{companyId:int}/{branchId:int}/{financialYearId:int}")]
        public async Task<ActionResult<List<AccountLedgerModel>>> GetSubLedgerByFinancialYear(int companyId, int branchId, int financialYearId)
        {
            try
            {
                await PopulateViewBag(financialYearId);

                return Ok(await _ledgerRepository.GetLedgerAsync(companyId, branchId, financialYearId));
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private async Task PopulateViewBag(int? selectedId = null)
        {
            var financialYears = await _financialYearRepository.GetAllActiveAsync();
        }
    }
}