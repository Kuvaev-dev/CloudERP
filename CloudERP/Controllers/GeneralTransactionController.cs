using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess;
using DatabaseAccess.Code;
using DatabaseAccess.Code.SP_Code;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class GeneralTransactionController : Controller
    {
        private readonly CloudDBEntities _db;
        private readonly SP_GeneralTransaction _accounts;
        private readonly GeneralTransactionEntry _generalEntry;
        private readonly ExchangeRateService _exchangeRateService;

        public GeneralTransactionController(CloudDBEntities db)
        {
            _db = db;
            _accounts = new SP_GeneralTransaction(_db);
            _generalEntry = new GeneralTransactionEntry(_db);
            _exchangeRateService = new ExchangeRateService(System.Configuration.ConfigurationManager.AppSettings["ExchangeRateApiKey"]);
        }

        // GET: GeneralTransaction/GeneralTransaction
        public async Task<ActionResult> GeneralTransaction()
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

                var rates = await _exchangeRateService.GetExchangeRatesAsync();
                ViewData["CurrencyRates"] = rates ?? new Dictionary<string, double>();

                var transaction = new GeneralTransactionMV();
                return View(transaction);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
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
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
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
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
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
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/SubJournal
        public ActionResult SubJournal()
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                int companyID = Convert.ToInt32(Session["CompanyID"]);
                int brnchID = Convert.ToInt32(Session["BrchID"]);

                var list = _accounts.GetJournal(companyID, brnchID, DateTime.Now, DateTime.Now);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
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
                int brnchID = Convert.ToInt32(Session["BrchID"]);

                var list = _accounts.GetJournal(companyID, brnchID, FromDate, ToDate);

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred while making changes: " + ex.Message;
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