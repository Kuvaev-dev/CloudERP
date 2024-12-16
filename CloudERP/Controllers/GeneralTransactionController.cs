using CloudERP.Helpers;
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
        private readonly SessionHelper _sessionHelper;

        public GeneralTransactionController(CloudDBEntities db, SP_GeneralTransaction accounts, GeneralTransactionEntry generalEntry, SessionHelper sessionHelper)
        {
            _db = db;
            _accounts = accounts;
            _generalEntry = generalEntry;
            _sessionHelper = sessionHelper;
        }

        // GET: GeneralTransaction/GeneralTransaction
        public ActionResult GeneralTransaction()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var accounts = _accounts.GetAllAccounts(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                ViewBag.CreditAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");
                ViewBag.DebitAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");

                return View(new GeneralTransactionMV());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                if (ModelState.IsValid)
                {
                    string payinvoiceno = "GEN" + DateTime.Now.ToString("yyyyMMddHHmmssmm");
                    var message = _generalEntry.ConfirmGeneralTransaction(transaction.TransferAmount, _sessionHelper.UserID, _sessionHelper.BranchID, _sessionHelper.CompanyID, payinvoiceno, transaction.DebitAccountControlID, transaction.CreditAccountControlID, transaction.Reason);

                    if (message.Contains(Resources.Common.Succeed) || message.Contains(Resources.Common.Succeed.ToLower()))
                    {
                        Session["GNMessage"] = message;
                        return RedirectToAction("Journal");
                    }
                    else
                    {
                        Session["GNMessage"] = Resources.Messages.PleaseReLoginAndTryAgain;
                    }
                }

                ViewBag.CreditAccountControlID = new SelectList(_accounts.GetAllAccounts(_sessionHelper.CompanyID, _sessionHelper.BranchID), "AccountSubControlID", "AccountSubControl", "0");
                ViewBag.DebitAccountControlID = new SelectList(_accounts.GetAllAccounts(_sessionHelper.CompanyID, _sessionHelper.BranchID), "AccountSubControlID", "AccountSubControl", "0");

                return RedirectToAction("GeneralTransaction", new { transaction });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/Journal
        public ActionResult Journal()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = _accounts.GetJournal(_sessionHelper.CompanyID, _sessionHelper.BranchID, DateTime.Now, DateTime.Now);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = _accounts.GetJournal(_sessionHelper.CompanyID, _sessionHelper.BranchID, FromDate, ToDate);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/SubJournal
        public ActionResult SubJournal()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = _accounts.GetJournal(_sessionHelper.CompanyID, _sessionHelper.BrchID, DateTime.Now, DateTime.Now);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var list = _accounts.GetJournal(_sessionHelper.CompanyID, _sessionHelper.BrchID, FromDate, ToDate);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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