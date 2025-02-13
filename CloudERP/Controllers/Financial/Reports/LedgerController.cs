using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class LedgerController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public LedgerController(SessionHelper sessionHelper)
        {
            _httpClient = new HttpClientHelper();
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> GetLedger()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var ledger = await _httpClient.GetAsync<AccountLedgerModel>(
                    $"ledger/branch-ledger?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(new List<AccountLedgerModel>());
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
                await PopulateViewBag(id);

                return View(await _httpClient.GetAsync<AccountLedgerModel>(
                    $"ledger/branch-ledger?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}"));
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

                var balanceSheet = await _httpClient.GetAsync<AccountLedgerModel>(
                    $"ledger/sub-branch-ledger?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
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
                await PopulateViewBag(id);

                return View(await _httpClient.GetAsync<BalanceSheetModel>($"balance-sheet/branch-balance-sheet?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}"));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag(int? selectedId = null)
        {
            var financialYears = await _httpClient.GetAsync<List<FinancialYear>>("financial-year/all");
            ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYearName", selectedId);
        }
    }
}