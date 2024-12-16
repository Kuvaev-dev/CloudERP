using CloudERP.Helpers;
using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class IncomeStatementController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly IncomeStatement _income;
        private readonly SessionHelper _sessionHelper;

        public IncomeStatementController(CloudDBEntities db, IncomeStatement income, SessionHelper sessionHelper)
        {
            _db = db;
            _income = income;
            _sessionHelper = sessionHelper;
        }

        // GET: IncomeStatement
        public ActionResult GetIncomeStatement()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var FinancialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = Resources.Messages.CompanyFinancialYearNotSet;
                    return View();
                }

                var incomeStatement = _income.GetIncomeStatement(_sessionHelper.CompanyID, _sessionHelper.BranchID, FinancialYear.FinancialYearID);

                return View(incomeStatement);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetIncomeStatement(int? FinancialYearID)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var incomeStatement = _income.GetIncomeStatement(_sessionHelper.CompanyID, _sessionHelper.BranchID, FinancialYearID ?? 0);

                return View(incomeStatement);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: IncomeStatement
        public ActionResult GetSubIncomeStatement()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var FinancialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = Resources.Messages.CompanyFinancialYearNotSet;
                    return View();
                }

                var incomeStatement = _income.GetIncomeStatement(_sessionHelper.CompanyID, _sessionHelper.BrchID, FinancialYear.FinancialYearID);

                return View(incomeStatement);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetSubIncomeStatement(int? FinancialYearID)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var incomeStatement = _income.GetIncomeStatement(_sessionHelper.CompanyID, _sessionHelper.BrchID, FinancialYearID ?? 0);

                return View(incomeStatement);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}