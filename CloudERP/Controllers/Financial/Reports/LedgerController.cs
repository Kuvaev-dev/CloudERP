using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Financial.Reports
{
    public class LedgerController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public LedgerController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        public async Task<ActionResult> GetLedger()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var ledger = await _httpClient.GetAsync<IEnumerable<AccountLedgerModel>>(
                    $"ledgerapi/getledger?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(ledger);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetLedger(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                return View(await _httpClient.GetAsync<IEnumerable<AccountLedgerModel>>(
                    $"ledgerapi/getledgerbyfinancialyear?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}"));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> GetSubLedger()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var balanceSheet = await _httpClient.GetAsync<IEnumerable<AccountLedgerModel>>(
                    $"ledgerapi/getledger?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetSubLedger(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                return View(await _httpClient.GetAsync<IEnumerable<AccountLedgerModel>>
                    ($"ledgerapi/getledgerbyfinancialyear?companyId={_sessionHelper.CompanyID}&branchId={id}"));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            ViewBag.FinancialYears = new SelectList(await _httpClient.GetAsync<List<FinancialYear>>(
                    "financialyearapi/getall"), "FinancialYearID", "FinancialYearName");
        }
    }
}