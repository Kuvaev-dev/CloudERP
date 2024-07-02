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

        public BalanceSheetController(CloudDBEntities db)
        {
            _db = db;
            _balSheet = new SP_BalanceSheet();
        }

        // GET: BalanceSheet
        public ActionResult GetBalanceSheet()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = GetCompanyID();
            int branchID = GetBranchID();

            try
            {
                var financialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (financialYear == null)
                {
                    ViewBag.Message = "Your Company Financial Year is not Set! Please Contact to Administrator!";
                    return View(new List<BalanceSheetModel>());
                }

                var balanceSheet = _balSheet.GetBalanceSheet(companyID, branchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetBalanceSheet(int? id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (!id.HasValue)
            {
                ViewBag.ErrorMessage = "Invalid Financial Year ID.";
                return View(new List<BalanceSheetModel>());
            }

            int companyID = GetCompanyID();
            int branchID = GetBranchID();

            try
            {
                var balanceSheet = _balSheet.GetBalanceSheet(companyID, branchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult GetSubBalanceSheet(string brnchid)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (!string.IsNullOrEmpty(brnchid))
            {
                Session["SubBranchID"] = brnchid;
            }

            int companyID = GetCompanyID();
            int branchID = GetSubBranchID();

            try
            {
                var financialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (financialYear == null)
                {
                    ViewBag.Message = "Your Company Financial Year is not Set! Please Contact to Administrator!";
                    return View(new List<BalanceSheetModel>());
                }

                var balanceSheet = _balSheet.GetBalanceSheet(companyID, branchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetSubBalanceSheet(int? id)
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Home");
            }

            if (!id.HasValue)
            {
                ViewBag.ErrorMessage = "Invalid Financial Year ID.";
                return View(new List<BalanceSheetModel>());
            }

            int companyID = GetCompanyID();
            int branchID = GetSubBranchID();

            try
            {
                var balanceSheet = _balSheet.GetBalanceSheet(companyID, branchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private bool IsUserLoggedIn()
        {
            return !string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"]));
        }

        private int GetCompanyID()
        {
            return Convert.ToInt32(Session["CompanyID"]);
        }

        private int GetBranchID()
        {
            return Convert.ToInt32(Session["BranchID"]);
        }

        private int GetSubBranchID()
        {
            return Convert.ToInt32(Session["SubBranchID"]);
        }
    }
}