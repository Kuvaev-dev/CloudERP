using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Financial.Reports
{
    public class BalanceSheetController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public BalanceSheetController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: BalanceSheet
        public async Task<ActionResult> GetBalanceSheet()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var balanceSheet = await _httpClient.GetAsync<BalanceSheetModel>(
                    $"balancesheetapi/getbalancesheet?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");

                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetBalanceSheet(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var balanceSheet = await _httpClient.GetAsync<BalanceSheetModel>(
                    $"balancesheetapi/getbalancesheetbyfinancialyear?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&financialYearId={id}");

                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> GetSubBalanceSheet(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();
                HttpContext.Session.SetInt32("SubBranchID", id);
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> PostSubBalanceSheet(int financialYearId)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();
                int? subBranchID = HttpContext.Session.GetInt32("SubBranchID");
                var balanceSheet = await _httpClient.GetAsync<BalanceSheetModel>(
                    $"balancesheetapi/getbalancesheetbyfinancialyear?companyId={_sessionHelper.CompanyID}" +
                    $"&branchId={subBranchID}&financialYearId={financialYearId}");
                return View("GetSubBalanceSheet", balanceSheet);
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
                    "financialyearapi/getall"), "FinancialYearID", "FinancialYearName");
        }
    }
}