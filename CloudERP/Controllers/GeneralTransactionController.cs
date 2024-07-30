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
        private readonly SP_GeneralTransaction _accounts;
        private readonly GeneralTransactionEntry _generalEntry;

        public GeneralTransactionController(CloudDBEntities db)
        {
            _db = db;
            _accounts = new SP_GeneralTransaction(_db);
            _generalEntry = new GeneralTransactionEntry(_db);
        }

        // GET: GeneralTransaction/GeneralTransaction
        public ActionResult GeneralTransaction()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var accounts = _accounts.GetAllAccounts(companyID, branchID);

                ViewBag.CreditAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");
                ViewBag.DebitAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");

                var transaction = new GeneralTransactionMV();
                return View(transaction);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: GeneralTransaction/SaveGeneralTransaction
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveGeneralTransaction(GeneralTransactionMV transaction)
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

                if (ModelState.IsValid)
                {
                    string payinvoiceno = "GEN" + DateTime.Now.ToString("yyyyMMddHHmmssmm");
                    var message = _generalEntry.ConfirmGeneralTransaction(transaction.TransferAmount, userID, branchID, companyID, payinvoiceno, transaction.DebitAccountControlID, transaction.CreditAccountControlID, transaction.Reason);

                    if (message.Contains("Succeed"))
                    {
                        Session["GNMessage"] = message;
                        return RedirectToAction("Journal");
                    }
                    else
                    {
                        Session["GNMessage"] = "Some issue occurred. Please re-login and try again!";
                    }
                }

                ViewBag.CreditAccountControlID = new SelectList(_accounts.GetAllAccounts(companyID, branchID), "AccountSubControlID", "AccountSubControl", "0");
                ViewBag.DebitAccountControlID = new SelectList(_accounts.GetAllAccounts(companyID, branchID), "AccountSubControlID", "AccountSubControl", "0");
                
                return RedirectToAction("GeneralTransaction", new { transaction });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/Journal
        public ActionResult Journal()
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

                var list = _accounts.GetJournal(companyID, branchID, DateTime.Now, DateTime.Now);

                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/SubJournal
        public ActionResult SubJournal(int? id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = (id != null) ? Convert.ToInt32(id) : Convert.ToInt32(Session["BrchID"]);
                var list = _accounts.GetJournal(companyID, branchID, DateTime.Now, DateTime.Now);

                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: GeneralTransaction/Journal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Journal(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var list = _accounts.GetJournal(companyID, branchID, FromDate, ToDate);

                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: GeneralTransaction/SubJournal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubJournal(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int branchID = Convert.ToInt32(Session["BranchID"]);

                var list = _accounts.GetJournal(companyID, branchID, FromDate, ToDate);

                return View(list);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}