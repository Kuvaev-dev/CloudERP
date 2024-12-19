using CloudERP.Helpers;
using DatabaseAccess.Models;
using DatabaseAccess.Repositories;
using Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class LedgerController : Controller
    {
        private readonly ILedgerRepository _ledgerRepository;
        private readonly IFinancialYearService _financialYearService;
        private readonly SessionHelper _sessionHelper;

        public LedgerController(ILedgerRepository ledgerRepository, IFinancialYearService financialYearService, SessionHelper sessionHelper)
        {
            _ledgerRepository = ledgerRepository;
            _financialYearService = financialYearService;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> GetLedger()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                await PopulateViewBag();

                var defaultFinancialYear = await _financialYearService.GetSingleActiveAsync();
                if (defaultFinancialYear != null)
                {
                    return View(await _ledgerRepository.GetLedgerAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, defaultFinancialYear.FinancialYearID));
                }

                return View(new List<AccountLedgerModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public async Task<ActionResult> GetLedger(int id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                await PopulateViewBagWithId(id);

                return View(await _ledgerRepository.GetLedgerAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, id));
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

        private async Task PopulateViewBagWithId(int id)
        {
            ViewBag.FinancialYears = new SelectList(await _financialYearService.GetAllActiveAsync(), "FinancialYearID", "FinancialYear", id);
        }
    }
}