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
        public async Task<ActionResult<IEnumerable<AccountLedgerModel>>> GetLedger(int companyId, int branchId)
        {
            try
            {
                var financialYear = await GetActiveAsync();

                var ledger = await _ledgerRepository.GetLedgerAsync(
                    companyId, 
                    branchId, 
                    financialYear.FinancialYearID);

                return Ok(ledger);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountLedgerModel>>> GetLedgerByFinancialYear(int companyId, int branchId, int financialYearId)
        {
            try
            {
                var ledger = await _ledgerRepository.GetLedgerAsync(
                   companyId,
                   branchId,
                   financialYearId);

                return Ok(ledger);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        private async Task<FinancialYear> GetActiveAsync() => await _financialYearRepository.GetSingleActiveAsync();
    }
}