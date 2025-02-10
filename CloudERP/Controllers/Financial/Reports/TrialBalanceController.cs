using CloudERP.Helpers;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TrialBalanceController : Controller
    {
        private readonly ITrialBalanceRepository _trialBalanceRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly SessionHelper _sessionHelper;

        public TrialBalanceController(
            ITrialBalanceRepository trialBalanceRepository, 
            IFinancialYearRepository financialYearRepository, 
            SessionHelper sessionHelper)
        {
            _trialBalanceRepository = trialBalanceRepository ?? throw new ArgumentNullException(nameof(ITrialBalanceRepository));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> GetTrialBalance()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var financialYear = await _financialYearRepository.GetSingleActiveAsync();

                var trialBalance = await _trialBalanceRepository.GetTrialBalanceAsync(
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    financialYear.FinancialYearID);

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

                var trialBalance = await _trialBalanceRepository.GetTrialBalanceAsync(
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    id ?? 0);

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
            var financialYears = await _financialYearRepository.GetAllActiveAsync();
            ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYearName", selectedId);
        }
    }
}