using DatabaseAccess.Code;
using DatabaseAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DatabaseAccess.Code.SP_Code;

namespace CloudERP.Controllers
{
    public class LedgerController : Controller
    {
        private CloudDBEntities db = new CloudDBEntities();
        private SP_Ledger ledgersp = new SP_Ledger();

        public ActionResult GetLedger()
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
            var ledger = ledgersp.GetLedger(companyID, branchID, FinancialYear.FinancialYearID);

            return View(ledger);
        }

        [HttpPost]
        public ActionResult GetLedger(int? id)
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

            var ledger = ledgersp.GetLedger(companyID, branchID, (int)id);

            return View(ledger);
        }
    }
}