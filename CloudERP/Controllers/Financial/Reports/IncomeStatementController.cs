using CloudERP.Helpers;
using Domain.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Financial.Reports
{
    public class IncomeStatementController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public IncomeStatementController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
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
                    $"income-statement/branch-income-statement/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}"));
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

                return View(await _httpClient.GetAsync<IncomeStatementModel>($"income-statement/branch-income-statement/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}"));
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
                    $"income-statement/sub-branch-income-statement/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}"));
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

                return View(await _httpClient.GetAsync<IncomeStatementModel>($"income-statement/sub-branch-income-statement/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}/{id}"));
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
                    "financial-year"), "FinancialYearID", "FinancialYearName");
        }
    }
}