using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TrialBalanceController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public TrialBalanceController(SessionHelper sessionHelper)
        {
            _httpClient = new HttpClientHelper();
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> GetTrialBalance()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var trialBalance = await _httpClient.GetAsync<TrialBalanceModel>(
                    $"trial-balance/branch-trial-balance?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

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
                await PopulateViewBag(id);

                var trialBalance = await _httpClient.GetAsync<BalanceSheetModel>(
                    $"trial-balance/branch-trial-balance?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}");

                return View(trialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag(int? selectedId = null)
        {
            ViewBag.FinancialYears = new SelectList(await _httpClient.GetAsync<List<FinancialYear>>(
                    "financial-year/all"), "FinancialYearID", "FinancialYearName");
        }
    }
}