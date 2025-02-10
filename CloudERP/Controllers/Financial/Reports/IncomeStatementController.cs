using CloudERP.Helpers;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class IncomeStatementController : Controller
    {
        private readonly IIncomeStatementService _income;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly SessionHelper _sessionHelper;

        public IncomeStatementController(
            IIncomeStatementService income, 
            IFinancialYearRepository financialYearRepository, 
            SessionHelper sessionHelper)
        {
            _income = income ?? throw new ArgumentNullException(nameof(IIncomeStatementService));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: IncomeStatement
        public async Task<ActionResult> GetIncomeStatement()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var FinancialYear = await _financialYearRepository.GetSingleActiveAsync();
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;
                    return View();
                }

                return View(await _income.GetIncomeStatementAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, FinancialYear.FinancialYearID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetIncomeStatement(int? FinancialYearID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                return View(await _income.GetIncomeStatementAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, FinancialYearID ?? 0));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: IncomeStatement
        public async Task<ActionResult> GetSubIncomeStatement()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var FinancialYear = await _financialYearRepository.GetSingleActiveAsync();
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = Localization.CloudERP.Messages.Messages.CompanyFinancialYearNotSet;
                    return View();
                }

                return View(new IncomeStatementModel());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetSubIncomeStatement(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                return View(await _income.GetIncomeStatementAsync(_sessionHelper.CompanyID, _sessionHelper.BrchID, id ?? 0));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            ViewBag.FinancialYears = new SelectList(await _financialYearRepository.GetAllActiveAsync(), "FinancialYearID", "FinancialYearName");
        }
    }
}