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

        [HttpGet, Route("branch-ledger/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetLedger(int companyId, int branchId)
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

        [HttpGet, Route("branch-ledger/{companyId:int}/{branchId:int}/{financialYearId:int}")]
        public async Task<IHttpActionResult> GetLedgerByFinancialYear(int companyId, int branchId, int financialYearId)
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

        [HttpGet, Route("sub-branch-ledger/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetSubLedger(int companyId, int branchId)
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

        [HttpGet, Route("sub-branch-ledger/{companyId:int}/{branchId:int}/{financialYearId:int}")]
        public async Task<IHttpActionResult> GetSubLedgerByFinancialYear(int companyId, int branchId, int financialYearId)
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