using DatabaseAccess;
using DatabaseAccess.Code;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class IncomeStatementController : Controller
    {
        private readonly CloudDBEntities db = new CloudDBEntities();
        private readonly IncomeStatement income = new IncomeStatement();

        // GET: IncomeStatement
        public ActionResult GetIncomeStatement()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var FinancialYear = db.tblFinancialYear.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message = "Your Company Financial Year is not Set! Please Contact to Administrator!";
            }
            var incomeStatement = income.GetIncomeStatement(companyID, branchID, FinancialYear.FinancialYearID);

            return View(incomeStatement);
        }

        [HttpPost]
        public ActionResult GetIncomeStatement(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var incomeStatement = income.GetIncomeStatement(companyID, branchID, (int)id);

            return View(incomeStatement);
        }

        public ActionResult GetSubIncomeStatement(string brchid)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            if (brchid != null)
            {
                Session["SubBranchID"] = brchid;
            }
            branchID = Convert.ToInt32(Convert.ToString(Session["SubBranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var FinancialYear = db.tblFinancialYear.Where(f => f.IsActive == true).FirstOrDefault();
            if (FinancialYear == null)
            {
                ViewBag.Message = "Your Company Financial Year is not Set! Please Contact to Administrator!";
            }
            var incomeStatement = income.GetIncomeStatement(companyID, branchID, FinancialYear.FinancialYearID);

            return View(incomeStatement);
        }

        [HttpPost]
        public ActionResult GetSubIncomeStatement(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["SubBranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));

            var incomeStatement = income.GetIncomeStatement(companyID, branchID, (int)id);

            return View(incomeStatement);
        }
    }
}