using CloudERP.Models;
using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class GeneralTransactionController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_GeneralTransaction accounts = new SP_GeneralTransaction();
        private readonly GeneralTransactionEntry generalEntry = new GeneralTransactionEntry();

        public GeneralTransactionController(CloudDBEntities db)
        {
            _db = db;
        }

        // GET: GeneralTransaction
        public ActionResult GeneralTransaction(GeneralTransactionMV transaction)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMessage"] != null)
            {
                Session["GNMessage"] = string.Empty;
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            ViewBag.CreditAccountControlID = new SelectList(accounts.GetAllAccounts(companyID, branchID), "AccountSubControlID", "AccountSubControl", "0");
            ViewBag.DebitAccountControlID = new SelectList(accounts.GetAllAccounts(companyID, branchID), "AccountSubControlID", "AccountSubControl", "0");
            return View(transaction);
        }

        public ActionResult SaveGeneralTransaction(GeneralTransactionMV transaction)
        {
            Session["GNMessage"] = string.Empty;
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

            if (ModelState.IsValid)
            {
                string payinvoiceno = "GEN" + DateTime.Now.ToString("yyyyMMddHHmmssmm");
                var message = generalEntry.ConfirmGeneralTransaction(transaction.TransferAmount, userID, branchID, companyID, payinvoiceno, transaction.DebitAccountControlID, transaction.CreditAccountControlID, transaction.Reason);
                
                if (message.Contains("Succeed"))
                {
                    Session["GNMessage"] = message;
                    return RedirectToAction("Journal");
                }
                else
                {
                    Session["GNMessage"] = "Some Issue is Occure, Re-Login and Try Again!";
                }
            }

            ViewBag.CreditAccountControlID = new SelectList(accounts.GetAllAccounts(companyID, branchID), "AccountSubControlID", "AccountSubControl", "0");
            ViewBag.DebitAccountControlID = new SelectList(accounts.GetAllAccounts(companyID, branchID), "AccountSubControlID", "AccountSubControl", "0");
            return RedirectToAction("GeneralTransactions", new { transaction });
        }

        public ActionResult Journal()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMessage"] != null)
            {
                Session["GNMessage"] = string.Empty;
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            branchID = Convert.ToInt32(Convert.ToString(Session["BranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = accounts.GetJournal(companyID, branchID, DateTime.Now, DateTime.Now);
            return View(list);
        }

        public ActionResult SubJournal(int? id)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }
            if (Session["GNMessage"] != null)
            {
                Session["GNMessage"] = string.Empty;
            }
            int companyID = 0;
            int branchID = 0;
            int userID = 0;
            if (id != null)
            {
                Session["SubBranchID"] = id;
            }
            branchID = Convert.ToInt32(Convert.ToString(Session["SubBranchID"]));
            companyID = Convert.ToInt32(Convert.ToString(Session["CompanyID"]));
            userID = Convert.ToInt32(Convert.ToString(Session["UserID"]));
            var list = accounts.GetJournal(companyID, branchID, DateTime.Now, DateTime.Now);
            return View(list);
        }

        [HttpPost]
        public ActionResult Journal(DateTime FromDate, DateTime ToDate)
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
            var list = accounts.GetJournal(companyID, branchID, FromDate, ToDate);
            return View(list);
        }

        [HttpPost]
        public ActionResult SubJournal(DateTime FromDate, DateTime ToDate)
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
            var list = accounts.GetJournal(companyID, branchID, FromDate, ToDate);
            return View(list);
        }
    }
}