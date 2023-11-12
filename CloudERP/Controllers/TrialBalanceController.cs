using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using DatabaseAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TrialBalanceController : Controller
    {
        CloudDBEntities db = new CloudDBEntities();
        SP_TrialBalance trialBalance = new SP_TrialBalance();

        // GET: TrialBalance
        public ActionResult GetTrialBalance()
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
            var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
            string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
            List<TrialBalanceModel> list = new List<TrialBalanceModel>();
            if (string.IsNullOrEmpty(FinancialYearID))
            {
                list = trialBalance.TriaBalance(branchID, companyID, 0);
            }
            else
            {
                list = trialBalance.TriaBalance(branchID, companyID, Convert.ToInt32(FinancialYearID));
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult GetTrialBalance(int? id)
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
            var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
            string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
            List<TrialBalanceModel> list = new List<TrialBalanceModel>();

            list = trialBalance.TriaBalance(branchID, companyID, (int)id);

            return View(list);
        }

        public ActionResult GetFinancialYear()
        {
            var getList = db.tblFinancialYear.ToList();
            var list = new List<tblFinancialYear>();
            foreach (var item in getList)
            {
                list.Add(new tblFinancialYear() { FinancialYearID = item.FinancialYearID, FinancialYear = item.FinancialYear });
            }

            return Json(new { data = list }, JsonRequestBehavior.AllowGet);
        }
    }
}