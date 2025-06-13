using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Financial.Reports
{
    public class TrialBalanceController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public TrialBalanceController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
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
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                await PopulateViewBag();

                var trialBalance = await _httpClient.GetAsync<IEnumerable<TrialBalanceModel>>(
                    $"trialbalanceapi/gettrialbalancebyfinancialyear?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}");

                return View(trialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> GetSubTrialBalance(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();
                HttpContext.Session.SetInt32("SubBranchID", id);
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostSubTrialBalance(int financialYearID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();
                int? subBranchID = HttpContext.Session.GetInt32("SubBranchID");
                var subTrialBalance = await _httpClient.GetAsync<IEnumerable<TrialBalanceModel>>(
                    $"trialbalanceapi/gettrialbalancebyfinancialyear?companyId={_sessionHelper.CompanyID}" +
                    $"&branchId={subBranchID}&financialYearId={financialYearID}");
                return View("GetSubTrialBalance", subTrialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            ViewBag.FinancialYears = new SelectList(await _httpClient.GetAsync<IEnumerable<FinancialYear>>(
                    "financialyearapi/getall"), "FinancialYearID", "FinancialYearName");
        }
    }
}