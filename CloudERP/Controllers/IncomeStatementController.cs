using CloudERP.Helpers;
using Domain.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class IncomeStatementController : Controller
    {
        private readonly IIncomeStatementService _income;
        private readonly IFinancialYearService _finYear;
        private readonly SessionHelper _sessionHelper;

        public IncomeStatementController(IIncomeStatementService income, IFinancialYearService finYear, SessionHelper sessionHelper)
        {
            _income = income;
            _finYear = finYear;
            _sessionHelper = sessionHelper;
        }

        // GET: IncomeStatement
        public async Task<ActionResult> GetIncomeStatement()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                await PopulateViewBag();

                var FinancialYear = await _finYear.GetSingleActiveAsync();
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = Resources.Messages.CompanyFinancialYearNotSet;
                    return View();
                }

                return View(await _income.GetIncomeStatementAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, FinancialYear.FinancialYearID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetIncomeStatement(int? FinancialYearID)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                await PopulateViewBag();

                return View(await _income.GetIncomeStatementAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, FinancialYearID ?? 0));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: IncomeStatement
        public async Task<ActionResult> GetSubIncomeStatement()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                await PopulateViewBag();

                var FinancialYear = await _finYear.GetSingleActiveAsync();
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = Resources.Messages.CompanyFinancialYearNotSet;
                    return View();
                }

                return View(await _income.GetIncomeStatementAsync(_sessionHelper.CompanyID, _sessionHelper.BrchID, FinancialYear.FinancialYearID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetSubIncomeStatement(int? FinancialYearID)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                await PopulateViewBag();

                return View(await _income.GetIncomeStatementAsync(_sessionHelper.CompanyID, _sessionHelper.BrchID, FinancialYearID ?? 0));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            ViewBag.FinancialYears = new SelectList(await _finYear.GetAllActiveAsync(), "FinancialYearID", "FinancialYear");
        }
    }
}