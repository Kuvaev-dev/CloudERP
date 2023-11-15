using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BalanceSheetController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();
        private SP_BalanceSheet bal_sheet = new SP_BalanceSheet();

        // GET: BalanceSheet
        public ActionResult BalanceSheet()
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
            var balanceSheet = bal_sheet.GetBalanceSheet(companyID, branchID, FinancialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });

            return View(balanceSheet);
        }
    }
}