using CloudERP.Helpers;
using CloudERP.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class GeneralTransactionController : Controller
    {
        private readonly IGeneralTransactionRepository _transactionRepository;
        private readonly SessionHelper _sessionHelper;

        public GeneralTransactionController(IGeneralTransactionRepository transactionRepository, SessionHelper sessionHelper)
        {
            _transactionRepository = transactionRepository;
            _sessionHelper = sessionHelper;
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
                    string payInvoiceNo = "GEN" + DateTime.Now.ToString("yyyyMMddHHmmssmm");
                    var message = await _transactionRepository.ConfirmGeneralTransaction(
                        transaction.TransferAmount,
                        _sessionHelper.UserID,
                        _sessionHelper.BranchID,
                        _sessionHelper.CompanyID,
                        payInvoiceNo,
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

                ViewBag.CreditAccountControlID = new SelectList(
                    await _transactionRepository.GetAllAccounts(_sessionHelper.CompanyID, _sessionHelper.BranchID),
                    "AccountSubControlID", "AccountSubControl", "0");

                ViewBag.DebitAccountControlID = new SelectList(
                    await _transactionRepository.GetAllAccounts(_sessionHelper.CompanyID, _sessionHelper.BranchID),
                    "AccountSubControlID", "AccountSubControl", "0");

                return RedirectToAction("GeneralTransaction", new { transaction });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}