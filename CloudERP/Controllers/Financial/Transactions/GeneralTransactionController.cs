using CloudERP.Helpers;
using CloudERP.Models;
using Domain.RepositoryAccess;
using Domain.Services;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class GeneralTransactionController : Controller
    {
        private readonly IGeneralTransactionService _generalTransactionService;
        private readonly IGeneralTransactionRepository _generalTransactionRepository;
        private readonly SessionHelper _sessionHelper;

        public GeneralTransactionController(
            IGeneralTransactionService generalTransactionService, 
            SessionHelper sessionHelper, 
            IGeneralTransactionRepository generalTransactionRepository)
        {
            _generalTransactionService = generalTransactionService ?? throw new ArgumentNullException(nameof(IGeneralTransactionService));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _generalTransactionRepository = generalTransactionRepository ?? throw new ArgumentNullException(nameof(IGeneralTransactionRepository));
        }

        // GET: GeneralTransaction/GeneralTransaction
        public async Task<ActionResult> GeneralTransaction()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var accounts = await _generalTransactionRepository.GetAllAccounts(_sessionHelper.CompanyID, _sessionHelper.BranchID);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveGeneralTransaction(GeneralTransactionMV transaction)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    var message = await _generalTransactionService.ConfirmTransactionAsync(
                        transaction.TransferAmount,
                        _sessionHelper.UserID,
                        _sessionHelper.BranchID,
                        _sessionHelper.CompanyID,
                        transaction.DebitAccountControlID,
                        transaction.CreditAccountControlID,
                        transaction.Reason);

                    if (message.Contains(Resources.Common.Succeed))
                    {
                        Session["GNMessage"] = message;
                        return RedirectToAction("Journal");
                    }

                    Session["GNMessage"] = Resources.Messages.PleaseReLoginAndTryAgain;
                }

                var accounts = await _generalTransactionRepository.GetAllAccounts(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                ViewBag.CreditAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl", "0");
                ViewBag.DebitAccountControlID = new SelectList(accounts, "AccountSubControlID", "AccountSubControl", "0");

                return RedirectToAction("GeneralTransaction", new { transaction });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/Journal
        public async Task<ActionResult> Journal()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _generalTransactionRepository.GetJournal(
                    _sessionHelper.CompanyID,
                    _sessionHelper.BranchID,
                    DateTime.Now,
                    DateTime.Now));
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
        public async Task<ActionResult> Journal(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _generalTransactionRepository.GetJournal(
                    _sessionHelper.CompanyID,
                    _sessionHelper.BranchID,
                    FromDate,
                    ToDate));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: GeneralTransaction/SubJournal
        public async Task<ActionResult> SubJournal()
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _generalTransactionRepository.GetJournal(
                    _sessionHelper.CompanyID, 
                    _sessionHelper.BrchID, 
                    DateTime.Now, 
                    DateTime.Now));
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
        public async Task<ActionResult> SubJournal(DateTime FromDate, DateTime ToDate)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                return View(await _generalTransactionRepository.GetJournal(
                    _sessionHelper.CompanyID,
                    _sessionHelper.BrchID,
                    FromDate,
                    ToDate));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}