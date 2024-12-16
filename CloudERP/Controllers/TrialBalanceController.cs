using CloudERP.Helpers;
using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TrialBalanceController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_TrialBalance _trialBalance;
        private readonly SessionHelper _sessionHelper;

        public TrialBalanceController(CloudDBEntities db, SP_TrialBalance trialBalance, SessionHelper sessionHelper)
        {
            _db = db;
            _trialBalance = trialBalance;
            _sessionHelper = sessionHelper;
        }

        // GET: TrialBalance
        public ActionResult GetTrialBalance()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");

                string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);

                List<TrialBalanceModel> list;

                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    list = _trialBalance.TrialBalance(_sessionHelper.BranchID, _sessionHelper.CompanyID, 0);
                }
                else
                {
                    list = _trialBalance.TrialBalance(_sessionHelper.BranchID, _sessionHelper.CompanyID, Convert.ToInt32(FinancialYearID));
                }

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetTrialBalance(int? id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                List<TrialBalanceModel> list;

                if (id.HasValue)
                {
                    list = _trialBalance.TrialBalance(_sessionHelper.BranchID, _sessionHelper.CompanyID, (int)id);
                }
                else
                {
                    list = new List<TrialBalanceModel>();
                }

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear", id);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}