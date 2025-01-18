using CloudERP.Helpers;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services.Purchase;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class PurchasePaymentReturnController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly IPurchaseReturnService _purchaseReturnService;
        private readonly IPurchasePaymentReturnService _purchasePaymentReturnService;
        private readonly IPurchaseRepository _purchaseRepository;

        public PurchasePaymentReturnController(
            IPurchaseReturnService purchaseReturnService,
            IPurchasePaymentReturnService purchasePaymentReturnService, 
            IPurchaseRepository purchaseRepository, 
            SessionHelper sessionHelper)
        {
            _purchaseReturnService = purchaseReturnService ?? throw new ArgumentNullException(nameof(IPurchaseReturnService));
            _purchasePaymentReturnService = purchasePaymentReturnService ?? throw new ArgumentNullException(nameof(IPurchasePaymentReturnService));
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(IPurchaseRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: PurchasePaymentReturn
        public async Task<ActionResult> ReturnPurchasePendingAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _purchaseRepository.PurchaseReturnPaymentPending(id));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllPurchasesPendingPayment()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _purchaseRepository.GetReturnPurchasesPaymentPending(_sessionHelper.CompanyID, _sessionHelper.BranchID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> ReturnAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null)
                {
                    return RedirectToAction("AllPurchasesPendingPayment");
                }

                var list = await _purchasePaymentReturnService.GetSupplierReturnPaymentsAsync((int)id);
                double remainingAmount = await _purchasePaymentReturnService.GetRemainingAmountAsync((int)id);

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReturnAmount(int? id, float previousRemainingAmount, float paymentAmount)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var returnAmountDto = new PurchaseReturnAmount
                {
                    InvoiceId = (int)id,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaymentAmount = paymentAmount
                };

                string message = await _purchasePaymentReturnService.ProcessReturnPaymentAsync(
                    returnAmountDto,
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    _sessionHelper.UserID);

                Session["Message"] = message;

                return RedirectToAction("PurchasePaymentReturn", new { id });
            }
            catch (InvalidOperationException ex)
            {
                ViewBag.Message = ex.Message;
                var list = await _purchasePaymentReturnService.GetSupplierReturnPaymentsAsync((int)id);
                double remainingAmount = await _purchasePaymentReturnService.GetRemainingAmountAsync((int)id);

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}