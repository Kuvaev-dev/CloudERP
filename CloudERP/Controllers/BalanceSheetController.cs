using CloudERP.Helpers;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BalanceSheetController : Controller
    {
        private readonly IBalanceSheetService _balanceSheetService;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly SessionHelper _sessionHelper;

        public BalanceSheetController(
            IBalanceSheetService balanceSheetService, 
            IFinancialYearRepository financialYearRepository, 
            SessionHelper sessionHelper)
        {
            _balanceSheetService = balanceSheetService ?? throw new ArgumentNullException(nameof(IBalanceSheetService));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: BalanceSheet
        public async Task<ActionResult> GetBalanceSheet()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var financialYear = await _financialYearRepository.GetSingleActiveAsync();
                if (financialYear == null)
                {
                    ViewBag.Message = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new BalanceSheetModel());
                }

                var balanceSheet = await _balanceSheetService.GetBalanceSheetAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetBalanceSheet(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!id.HasValue)
            {
                ViewBag.ErrorMessage = Resources.Messages.InvalidFinancialYearID;
                return View(new BalanceSheetModel());
            }

            try
            {
                await PopulateViewBag();

                var balanceSheet = await _balanceSheetService.GetBalanceSheetAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> GetSubBalanceSheet()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                var financialYear = await _financialYearRepository.GetSingleActiveAsync();
                if (financialYear == null)
                {
                    ViewBag.Message = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new List<BalanceSheetModel>());
                }

                var balanceSheet = await _balanceSheetService.GetBalanceSheetAsync(_sessionHelper.CompanyID, _sessionHelper.BrchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetSubBalanceSheet(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!id.HasValue)
            {
                ViewBag.ErrorMessage = Resources.Messages.InvalidFinancialYearID;
                return View(new List<BalanceSheetModel>());
            }

            try
            {
                await PopulateViewBag();

                var balanceSheet = await _balanceSheetService.GetBalanceSheetAsync(_sessionHelper.CompanyID, _sessionHelper.BrchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
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
    }
}