using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/balance-sheet")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BalanceSheetApiController : ApiController
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
        [HttpGet, Route("branch-balance-sheet/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetBalanceSheet(int companyId, int branchId)
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
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("branch-balance-sheet/{companyId:int}/{branchId:int}/{financialYearId:int}")]
        public async Task<IHttpActionResult> GetBalanceSheetByFinancialYear(int companyId, int branchId, int financialYearId)
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
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("sub-branch-balance-sheet/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetSubBalanceSheet(int companyId, int branchId)
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
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("branch-balance-sheet/{companyId:int}/{branchId:int}/{financialYearId:int}")]
        public async Task<IHttpActionResult> GetSubBalanceSheetByFinancialYear(int companyId, int branchId, int financialYearId)
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
                return InternalServerError(ex);
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