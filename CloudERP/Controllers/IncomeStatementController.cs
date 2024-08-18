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

        public IncomeStatementController(CloudDBEntities db)
        {
            _db = db;
            _income = new IncomeStatement(_db);
        }

        // GET: IncomeStatement
        public ActionResult GetIncomeStatement()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var FinancialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = Resources.Messages.CompanyFinancialYearNotSet;
                    return View();
                }

                var incomeStatement = _income.GetIncomeStatement(companyID, branchID, FinancialYear.FinancialYearID);

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
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var incomeStatement = _income.GetIncomeStatement(companyID, branchID, FinancialYearID ?? 0);

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
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int brchID = Convert.ToInt32(Session["BrchID"]);

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var FinancialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = Resources.Messages.CompanyFinancialYearNotSet;
                    return View();
                }

                var incomeStatement = _income.GetIncomeStatement(companyID, brchID, FinancialYear.FinancialYearID);

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
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int brchID = Convert.ToInt32(Session["BrchID"]);

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var incomeStatement = _income.GetIncomeStatement(companyID, brchID, FinancialYearID ?? 0);

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