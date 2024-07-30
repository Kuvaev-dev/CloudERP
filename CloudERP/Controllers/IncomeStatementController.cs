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
                    ViewBag.ErrorMessage = "Your company's financial year is not set. Please contact the Administrator.";
                    return View();
                }

                var incomeStatement = _income.GetIncomeStatement(companyID, branchID, FinancialYear.FinancialYearID);

                return View(incomeStatement);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetIncomeStatement(int? id)
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
                    ViewBag.ErrorMessage = "Your company's financial year is not set. Please contact the Administrator.";
                    return View();
                }

                var incomeStatement = _income.GetIncomeStatement(companyID, branchID, (int)id);

                return View(incomeStatement);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}