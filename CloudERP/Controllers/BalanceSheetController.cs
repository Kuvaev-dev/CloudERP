using CloudERP.Helpers;
using DatabaseAccess;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BalanceSheetController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_BalanceSheet _balSheet;
        private readonly SessionHelper _sessionHelper;

        public BalanceSheetController(CloudDBEntities db, SP_BalanceSheet balSheet, SessionHelper sessionHelper)
        {
            _db = db;
            _balSheet = balSheet;
            _sessionHelper = sessionHelper;
        }

        // GET: BalanceSheet
        public ActionResult GetBalanceSheet()
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var financialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (financialYear == null)
                {
                    ViewBag.Message = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new BalanceSheetModel());
                }

                var balanceSheet = _balSheet.GetBalanceSheet(_sessionHelper.CompanyID, _sessionHelper.BranchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetBalanceSheet(int? id)
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
                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var balanceSheet = _balSheet.GetBalanceSheet(_sessionHelper.CompanyID, _sessionHelper.BranchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult GetSubBalanceSheet()
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var financialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (financialYear == null)
                {
                    ViewBag.Message = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new List<BalanceSheetModel>());
                }

                var balanceSheet = _balSheet.GetBalanceSheet(_sessionHelper.CompanyID, _sessionHelper.BrchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetSubBalanceSheet(int? id)
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
                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var balanceSheet = _balSheet.GetBalanceSheet(_sessionHelper.CompanyID, _sessionHelper.BrchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}