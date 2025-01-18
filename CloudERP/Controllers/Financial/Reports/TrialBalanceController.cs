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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
            ViewBag.FinancialYears = new SelectList(await _financialYearRepository.GetAllActiveAsync(), "FinancialYearID", "FinancialYear");
        }

        private async Task PopulateViewBagWithId(int? id)
        {
            ViewBag.FinancialYears = new SelectList(await _financialYearRepository.GetAllActiveAsync(), "FinancialYearID", "FinancialYear", id);
        }
    }
}