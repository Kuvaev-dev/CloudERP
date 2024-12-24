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
        private readonly IBalanceSheetRepository _balSheet;
        private readonly IFinancialYearService _financialYearService;
        private readonly SessionHelper _sessionHelper;

        public BalanceSheetController(IBalanceSheetRepository balSheet, IFinancialYearService financialYearService, SessionHelper sessionHelper)
        {
            _balSheet = balSheet;
            _financialYearService = financialYearService;
            _sessionHelper = sessionHelper;
        }

        // GET: BalanceSheet
        public async Task<ActionResult> GetBalanceSheet()
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                await PopulateViewBag();

                var financialYear = await _financialYearService.GetSingleActiveAsync();
                if (financialYear == null)
                {
                    ViewBag.Message = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new BalanceSheetModel());
                }

                var balanceSheet = await _balSheet.GetBalanceSheetAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
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
            {
                return RedirectToAction("Login", "Home");
            }

            if (!id.HasValue)
            {
                ViewBag.ErrorMessage = Resources.Messages.InvalidFinancialYearID;
                return View(new BalanceSheetModel());
            }

            try
            {
                await PopulateViewBag();

                var balanceSheet = await _balSheet.GetBalanceSheetAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
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
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                await PopulateViewBag();

                var financialYear = await _financialYearService.GetSingleActiveAsync();
                if (financialYear == null)
                {
                    ViewBag.Message = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new List<BalanceSheetModel>());
                }

                var balanceSheet = await _balSheet.GetBalanceSheetAsync(_sessionHelper.CompanyID, _sessionHelper.BrchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
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
            {
                return RedirectToAction("Login", "Home");
            }

            if (!id.HasValue)
            {
                ViewBag.ErrorMessage = Resources.Messages.InvalidFinancialYearID;
                return View(new List<BalanceSheetModel>());
            }

            try
            {
                await PopulateViewBag();

                var balanceSheet = await _balSheet.GetBalanceSheetAsync(_sessionHelper.CompanyID, _sessionHelper.BrchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
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
            ViewBag.FinancialYears = new SelectList(await _financialYearService.GetAllActiveAsync(), "FinancialYearID", "FinancialYear");
        }
    }
}