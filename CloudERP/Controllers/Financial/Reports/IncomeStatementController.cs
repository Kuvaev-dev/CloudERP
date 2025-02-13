using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class IncomeStatementController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public IncomeStatementController(SessionHelper sessionHelper)
        {
            _httpClient = new HttpClientHelper();
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

                return View(_httpClient.GetAsync<IncomeStatementModel>(
                    $"income-statement/branch-income-statement?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}"));
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

            if (!FinancialYearID.HasValue)
            {
                ViewBag.ErrorMessage = Localization.CloudERP.Messages.Messages.InvalidFinancialYearID;
                return View(new IncomeStatementModel());
            }

            try
            {
                await PopulateViewBag();

                return View(await _httpClient.GetAsync<IncomeStatementModel>($"income-statement/branch-income-statement?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}"));
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

                return View(_httpClient.GetAsync<IncomeStatementModel>(
                    $"income-statement/sub-branch-income-statement?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}"));
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

                return View(await _httpClient.GetAsync<IncomeStatementModel>($"income-statement/sub-branch-income-statement?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}"));
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
                    "financial-year/all"), "FinancialYearID", "FinancialYearName");
        }
    }
}