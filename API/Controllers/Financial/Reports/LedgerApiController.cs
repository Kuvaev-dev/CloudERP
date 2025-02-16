using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/ledger")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LedgerApiController : ApiController
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

        [HttpGet, Route("branch-ledger?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetLedger([FromUri] int companyId, [FromUri] int branchId)
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
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("branch-ledger?companyId={companyId:int}&branchId={branchId:int}&financialYearId={financialYearId:int}")]
        public async Task<IHttpActionResult> GetLedgerByFinancialYear([FromUri] int companyId, [FromUri] int branchId, [FromUri] int financialYearId)
        {
            try
            {
                await PopulateViewBag(financialYearId);

                return Ok(await _ledgerRepository.GetLedgerAsync(companyId, branchId, financialYearId));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("sub-branch-ledger?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetSubLedger([FromUri] int companyId, [FromUri] int branchId)
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
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("sub-branch-ledger?companyId={companyId:int}&branchId={branchId:int}&financialYearId={financialYearId:int}")]
        public async Task<IHttpActionResult> GetSubLedgerByFinancialYear([FromUri] int companyId, [FromUri] int branchId, [FromUri] int financialYearId)
        {
            try
            {
                await PopulateViewBag(financialYearId);

                return Ok(await _ledgerRepository.GetLedgerAsync(companyId, branchId, financialYearId));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private async Task PopulateViewBag(int? selectedId = null)
        {
            var financialYears = await _financialYearRepository.GetAllActiveAsync();
        }
    }
}