using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Financial.Transactions
{
    public class GeneralTransactionController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        public GeneralTransactionController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
        }

        // GET: GeneralTransaction/GeneralTransaction
        public async Task<ActionResult> GeneralTransaction()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();
                return View(new GeneralTransactionMV());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveGeneralTransaction(GeneralTransactionMV transaction)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid)
            {
                await PopulateViewBag();
                return View("GeneralTransaction", transaction);
            }

            try
            {
                var endpoint = $"generaltransactionapi/savetransaction?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&userId={_sessionHelper.UserID}";
                var result = await _httpClient.PostAsync(endpoint, transaction);

                if (result)
                {
                    HttpContext.Session.SetString("GNMessage", "Transaction succeeded.");
                    return RedirectToAction("Journal");
                }

                HttpContext.Session.SetString("GNMessage", "Failed to save transaction. Please try again.");
                await PopulateViewBag();
                return View("GeneralTransaction", transaction);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/Journal
        public async Task<ActionResult> Journal()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var endpoint = $"generaltransactionapi/getjournal?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&fromDate={DateTime.Now:yyyy-MM-dd}&toDate={DateTime.Now:yyyy-MM-dd}";
                var journalEntries = await _httpClient.GetAsync<JournalModel>(endpoint);
                return View(journalEntries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: GeneralTransaction/Journal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Journal(DateTime FromDate, DateTime ToDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var endpoint = $"generaltransactionapi/getjournal?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}";
                var journalEntries = await _httpClient.GetAsync<JournalModel>(endpoint);
                return View(journalEntries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/SubJournal
        public async Task<ActionResult> SubJournal(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var endpoint = $"generaltransactionapi/getjournal?companyId={_sessionHelper.CompanyID}&branchId={id ?? _sessionHelper.BranchID}&fromDate={DateTime.Now:yyyy-MM-dd}&toDate={DateTime.Now:yyyy-MM-dd}";
                var subJournalEntries = await _httpClient.GetAsync<JournalModel>(endpoint);
                return View(subJournalEntries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: GeneralTransaction/SubJournal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubJournal(DateTime FromDate, DateTime ToDate, int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var endpoint = $"generaltransactionapi/getjournal?companyId={_sessionHelper.CompanyID}&branchId={id ?? _sessionHelper.BranchID}&fromDate{FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}";
                var subJournalEntries = await _httpClient.GetAsync<object>(endpoint);
                return View(subJournalEntries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            var endpoint = $"generaltransactionapi/getaccounts?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}";
            var accounts = await _httpClient.GetAsync<List<AllAccountModel>>(endpoint);

            ViewBag.CreditAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");
            ViewBag.DebitAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");
        }
    }
}