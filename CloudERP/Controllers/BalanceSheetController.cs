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
            _balSheet = new SP_BalanceSheet(_db);
        }

        // GET: BalanceSheet
        public ActionResult GetBalanceSheet()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            try
            {
                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var financialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (financialYear == null)
                {
                    ViewBag.Message = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new BalanceSheetModel());
                }

                var balanceSheet = _balSheet.GetBalanceSheet(companyID, branchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetBalanceSheet(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (!id.HasValue)
            {
                ViewBag.ErrorMessage = "Invalid Financial Year ID.";
                return View(new BalanceSheetModel());
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            try
            {
                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var balanceSheet = _balSheet.GetBalanceSheet(companyID, branchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult GetSubBalanceSheet()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int brchID = Convert.ToInt32(Session["BrchID"]);

            try
            {
                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var financialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (financialYear == null)
                {
                    ViewBag.Message = Resources.Messages.CompanyFinancialYearNotSet;
                    return View(new List<BalanceSheetModel>());
                }

                var balanceSheet = _balSheet.GetBalanceSheet(companyID, brchID, financialYear.FinancialYearID, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetSubBalanceSheet(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            if (!id.HasValue)
            {
                ViewBag.ErrorMessage = Resources.Messages.InvalidFinancialYearID;
                return View(new List<BalanceSheetModel>());
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);
            int brchID = Convert.ToInt32(Session["BrchID"]);

            try
            {
                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var balanceSheet = _balSheet.GetBalanceSheet(companyID, brchID, id.Value, new List<int> { 1, 2, 3, 4, 5 });
                return View(balanceSheet);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}