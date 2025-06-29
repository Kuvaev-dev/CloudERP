﻿using CloudERP.Models;
using Domain.Models.FinancialModels;
using Domain.UtilsAccess;
using Localization.CloudERP.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Financial.Transactions
{
    public class GeneralTransactionController : Controller
    {
        private readonly ISessionHelper _sessionHelper;
        private readonly IHttpClientHelper _httpClient;

        public GeneralTransactionController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

            if (transaction.CreditAccountControlID == transaction.DebitAccountControlID)
            {
                ViewBag.ErrorMessage = Messages.GeneralTransactionSelectionError;
                await PopulateViewBag();
                return View("GeneralTransaction", transaction);
            }

            try
            {
                var endpoint = $"generaltransactionapi/savetransaction?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&userId={_sessionHelper.UserID}";
                var result = await _httpClient.PostAsync(endpoint, transaction);
                if (result) return RedirectToAction("Journal");

                await PopulateViewBag();
                return View("GeneralTransaction", transaction);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                var journalEntries = await _httpClient.GetAsync<IEnumerable<JournalModel>>(endpoint);
                return View(journalEntries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                var journalEntries = await _httpClient.GetAsync<IEnumerable<JournalModel>>(endpoint);
                return View(journalEntries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/SubJournal
        public ActionResult SubJournal(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            HttpContext.Session.SetInt32("SubBranchID", id);

            return View();
        }

        // POST: GeneralTransaction/SubJournal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubJournal(DateTime FromDate, DateTime ToDate)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                int? subBranchID = HttpContext.Session.GetInt32("SubBranchID");
                var subJournalEntries = await _httpClient.GetAsync<IEnumerable<JournalModel>>(
                    $"generaltransactionapi/getjournal?companyId={_sessionHelper.CompanyID}&branchId={subBranchID}" +
                    $"&fromDate{FromDate:yyyy-MM-dd}&toDate={ToDate:yyyy-MM-dd}");
                return View(subJournalEntries);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            var endpoint = $"generaltransactionapi/getaccounts?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}";
            var accounts = await _httpClient.GetAsync<IEnumerable<AllAccountModel>>(endpoint);

            ViewBag.CreditAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");
            ViewBag.DebitAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl");
        }
    }
}