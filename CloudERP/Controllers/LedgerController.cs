using DatabaseAccess;
using DatabaseAccess.Code.SP_Code;
using System;
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

                var FinancialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = "Your company's financial year is not set. Please contact the Administrator.";
                    return View();
                }

                var ledger = _ledgersp.GetLedger(companyID, branchID, FinancialYear.FinancialYearID);
                return View(ledger);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        public ActionResult GetLedger(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var ledger = _ledgersp.GetLedger(companyID, branchID, (int)id);

                return View(ledger);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult GetSubLedger(string brchid)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(brchid); // Assuming brchid is a string representation of branch ID

                var FinancialYear = _db.tblFinancialYear.FirstOrDefault(f => f.IsActive);
                if (FinancialYear == null)
                {
                    ViewBag.ErrorMessage = "Your company's financial year is not set. Please contact the Administrator.";
                    return View();
                }

                var ledger = _ledgersp.GetLedger(companyID, branchID, FinancialYear.FinancialYearID);

                return View(ledger);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
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
                int branchID = Convert.ToInt32(Session["SubBranchID"]);

                var ledger = _ledgersp.GetLedger(companyID, branchID, (int)id);

                return View(ledger);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}