using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Financial.Reports
{
    public class TrialBalanceController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public TrialBalanceController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> GetTrialBalance()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var trialBalance = await _httpClient.GetAsync<IEnumerable<TrialBalanceModel>>(
                    $"trialbalanceapi/gettrialbalance?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(trialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetTrialBalance(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag(id);

                var trialBalance = await _httpClient.GetAsync<IEnumerable<BalanceSheetModel>>(
                    $"trialbalanceapi/gettrialbalancebyfinancialyear?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}");

                return View(trialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> GetSubTrialBalance()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var trialBalance = await _httpClient.GetAsync<IEnumerable<TrialBalanceModel>>(
                    $"trialbalanceapi/gettrialbalance?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(trialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetSubTrialBalance(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag(id);

                var trialBalance = await _httpClient.GetAsync<IEnumerable<BalanceSheetModel>>(
                    $"trialbalanceapi/gettrialbalancebyfinancialyear?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}");

                return View(trialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag(int? selectedId = null)
        {
            ViewBag.FinancialYears = new SelectList(await _httpClient.GetAsync<IEnumerable<FinancialYear>>(
                    "financialyearapi/getall"), "FinancialYearID", "FinancialYearName");
        }
    }
}