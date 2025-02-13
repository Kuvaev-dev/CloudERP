using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class GeneralTransactionController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClientHelper;

        public GeneralTransactionController(SessionHelper sessionHelper)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _httpClientHelper = new HttpClientHelper();
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
                var endpoint = $"save-transaction?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&userId={_sessionHelper.UserID}";
                var result = await _httpClientHelper.PostAsync(endpoint, transaction);

                if (result)
                {
                    Session["GNMessage"] = "Transaction succeeded.";
                    return RedirectToAction("Journal");
                }

                Session["GNMessage"] = "Failed to save transaction. Please try again.";
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
                var endpoint = $"journal?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&fromDate={DateTime.Now:yyyy-MM-dd}&toDate={DateTime.Now:yyyy-MM-dd}";
                var journalEntries = await _httpClientHelper.GetAsync<JournalModel>(endpoint);
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
                var endpoint = $"journal?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}";
                var journalEntries = await _httpClientHelper.GetAsync<JournalModel>(endpoint);
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
                var endpoint = $"sub-journal/{id ?? _sessionHelper.BranchID}?companyId={_sessionHelper.CompanyID}&fromDate={DateTime.Now:yyyy-MM-dd}&toDate={DateTime.Now:yyyy-MM-dd}";
                var subJournalEntries = await _httpClientHelper.GetAsync<JournalModel>(endpoint);
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
                var endpoint = $"sub-journal/{id ?? _sessionHelper.BranchID}?companyId={_sessionHelper.CompanyID}&fromDate={FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}";
                var subJournalEntries = await _httpClientHelper.GetAsync<object>(endpoint);
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
            var endpoint = $"accounts?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}";
            var accounts = await _httpClientHelper.GetAsync<List<AllAccountModel>>(endpoint);

            ViewBag.CreditAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");
            ViewBag.DebitAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");
        }
    }
}