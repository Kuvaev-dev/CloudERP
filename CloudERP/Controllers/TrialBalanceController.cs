using CloudERP.Helpers;
using DatabaseAccess.Repositories;
using Domain.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TrialBalanceController : Controller
    {
        private readonly ITrialBalanceRepository _trialBalanceRepository;
        private readonly IFinancialYearService _financialYearService;
        private readonly SessionHelper _sessionHelper;

        public TrialBalanceController(ITrialBalanceRepository trialBalanceRepository, IFinancialYearService financialYearService, SessionHelper sessionHelper)
        {
            _trialBalanceRepository = trialBalanceRepository;
            _financialYearService = financialYearService;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> GetTrialBalance()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                await PopulateViewBag();

                var financialYear = await _financialYearService.GetSingleActiveAsync();

                var trialBalance = await _trialBalanceRepository.GetTrialBalanceAsync(
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    financialYear.FinancialYearID);

                return View(trialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetTrialBalance(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                await PopulateViewBagWithId(id);

                var trialBalance = await _trialBalanceRepository.GetTrialBalanceAsync(
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    id ?? 0);

                return View(trialBalance);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            ViewBag.FinancialYears = new SelectList(await _financialYearService.GetAllActiveAsync(), "FinancialYearID", "FinancialYear");
        }

        private async Task PopulateViewBagWithId(int? id)
        {
            ViewBag.FinancialYears = new SelectList(await _financialYearService.GetAllActiveAsync(), "FinancialYearID", "FinancialYear", id);
        }
    }
}