using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Financial.Reports
{
    public class IncomeStatementController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public IncomeStatementController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: IncomeStatement
        public async Task<ActionResult> GetIncomeStatement()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                return View(await _httpClient.GetAsync<IncomeStatementModel>(
                    $"incomestatementapi/getincomestatement?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}"));
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

                return View(await _httpClient.GetAsync<IncomeStatementModel>(
                    $"incomestatementapi/getincomestatementbyfinancialyear?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&FinancialYearID={FinancialYearID}"));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: IncomeStatement
        public async Task<ActionResult> GetSubIncomeStatement(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();
                HttpContext.Session.SetInt32("SubBranchID", id);
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
        public async Task<ActionResult> PostSubIncomeStatement(int financialYearID)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();
                int? subBranchID = HttpContext.Session.GetInt32("SubBranchID");
                var subIncome = await _httpClient.GetAsync<IncomeStatementModel>(
                    $"incomestatementapi/getincomestatementbyfinancialyear?companyId={_sessionHelper.CompanyID}" +
                    $"&branchId={subBranchID}&FinancialYearID={financialYearID}");
                return View("GetSubIncomeStatement", subIncome);
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