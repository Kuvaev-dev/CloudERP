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

        public TrialBalanceController(CloudDBEntities db)
        {
            _db = db;
            _trialBalance = new SP_TrialBalance();
        }

        // GET: TrialBalance
        public ActionResult GetTrialBalance()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                
                string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                
                List<TrialBalanceModel> list = new List<TrialBalanceModel>();
                
                if (string.IsNullOrEmpty(FinancialYearID))
                {
                    list = _trialBalance.TriaBalance(branchID, companyID, 0);
                }
                else
                {
                    list = _trialBalance.TriaBalance(branchID, companyID, Convert.ToInt32(FinancialYearID));
                }

                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetTrialBalance(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);
                int userID = Convert.ToInt32(Session["UserID"]);

                var financialYearCheck = DatabaseQuery.Retrive("select top 1 FinancialYearID from tblFinancialYear where IsActive = 1");
                
                string FinancialYearID = (financialYearCheck != null ? Convert.ToString(financialYearCheck.Rows[0][0]) : string.Empty);
                
                List<TrialBalanceModel> list = new List<TrialBalanceModel>();

                list = _trialBalance.TriaBalance(branchID, companyID, (int)id);

                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult GetFinancialYear()
        {
            try
            {
                var getList = _db.tblFinancialYear.ToList();

                var list = new List<tblFinancialYear>();

                foreach (var item in getList)
                {
                    list.Add(new tblFinancialYear() { FinancialYearID = item.FinancialYearID, FinancialYear = item.FinancialYear });
                }

                return Json(new { data = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}