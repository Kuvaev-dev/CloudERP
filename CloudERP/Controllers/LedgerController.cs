using DatabaseAccess;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class LedgerController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_Ledger _ledgersp;

        public LedgerController(CloudDBEntities db)
        {
            _db = db;
            _ledgersp = new SP_Ledger(_db);
        }

        public ActionResult GetLedger()
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
                if (!financialYears.Any())
                {
                    ViewBag.ErrorMessage = "Your company's financial years are not set. Please contact the Administrator.";
                    return View(new List<DatabaseAccess.Models.AccountLedgerModel>());
                }

                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var defaultFinancialYear = financialYears.FirstOrDefault();
                if (defaultFinancialYear != null)
                {
                    var ledger = _ledgersp.GetLedger(companyID, branchID, defaultFinancialYear.FinancialYearID);
                    return View(ledger);
                }

                return View(new List<DatabaseAccess.Models.AccountLedgerModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetLedger(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var ledger = _ledgersp.GetLedger(companyID, branchID, id);

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear", id);

                return View(ledger);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult GetSubLedger()
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
                if (!financialYears.Any())
                {
                    ViewBag.ErrorMessage = "Your company's financial years are not set. Please contact the Administrator.";
                    return View(new List<DatabaseAccess.Models.AccountLedgerModel>());
                }

                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear");

                var defaultFinancialYear = financialYears.FirstOrDefault();
                if (defaultFinancialYear != null)
                {
                    var ledger = _ledgersp.GetLedger(companyID, branchID, defaultFinancialYear.FinancialYearID);
                    return View(ledger);
                }

                return View(new List<DatabaseAccess.Models.AccountLedgerModel>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetSubLedger(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var ledger = _ledgersp.GetLedger(companyID, branchID, id ?? 0);

                var financialYears = _db.tblFinancialYear.Where(f => f.IsActive).ToList();
                ViewBag.FinancialYears = new SelectList(financialYears, "FinancialYearID", "FinancialYear", id);

                return View(ledger);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}