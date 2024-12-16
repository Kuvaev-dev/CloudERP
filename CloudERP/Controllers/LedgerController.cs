using CloudERP.Helpers;
using DatabaseAccess;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class LedgerController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_Ledger _ledgersp;
        private readonly SessionHelper _sessionHelper;

        public LedgerController(CloudDBEntities db, SP_Ledger ledgersp, SessionHelper sessionHelper)
        {
            _db = db;
            _ledgersp = ledgersp;
            _sessionHelper = sessionHelper;
        }

        public ActionResult GetLedger()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                if (!financialYears.Any())
                {
                    ViewBag.ErrorMessage = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new List<DatabaseAccess.Models.AccountLedgerModel>());
                }

                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var defaultFinancialYear = financialYears.FirstOrDefault();
                if (defaultFinancialYear != null)
                {
                    var ledger = _ledgersp.GetLedger(_sessionHelper.CompanyID, _sessionHelper.BranchID, defaultFinancialYear.FinancialYearID);
                    return View(ledger);
                }

                return View(new List<DatabaseAccess.Models.AccountLedgerModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetLedger(int id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var ledger = _ledgersp.GetLedger(_sessionHelper.CompanyID, _sessionHelper.BranchID, id);

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear", id);

                return View(ledger);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult GetSubLedger()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                if (!financialYears.Any())
                {
                    ViewBag.ErrorMessage = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new List<DatabaseAccess.Models.AccountLedgerModel>());
                }

                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var defaultFinancialYear = financialYears.FirstOrDefault();
                if (defaultFinancialYear != null)
                {
                    var ledger = _ledgersp.GetLedger(_sessionHelper.CompanyID, _sessionHelper.BranchID, defaultFinancialYear.FinancialYearID);
                    return View(ledger);
                }

                return View(new List<DatabaseAccess.Models.AccountLedgerModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetSubLedger(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var ledger = _ledgersp.GetLedger(_sessionHelper.CompanyID, _sessionHelper.BranchID, id ?? 0);

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear", id);

                return View(ledger);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}