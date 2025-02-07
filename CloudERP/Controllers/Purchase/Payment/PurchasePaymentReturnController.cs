using CloudERP.Helpers;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.Services;
using System;
using System.Linq;
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
        private readonly ISupplierReturnPaymentRepository _supplierReturnPaymentRepository;

        public PurchasePaymentReturnController(
            IPurchaseReturnService purchaseReturnService,
            IPurchasePaymentReturnService purchasePaymentReturnService, 
            IPurchaseRepository purchaseRepository,
            ISupplierReturnPaymentRepository supplierReturnPaymentRepository,
            SessionHelper sessionHelper)
        {
            _purchaseReturnService = purchaseReturnService ?? throw new ArgumentNullException(nameof(IPurchaseReturnService));
            _purchasePaymentReturnService = purchasePaymentReturnService ?? throw new ArgumentNullException(nameof(IPurchasePaymentReturnService));
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(IPurchaseRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _supplierReturnPaymentRepository = supplierReturnPaymentRepository ?? throw new ArgumentNullException(nameof(ISupplierReturnPaymentRepository));
        }

        // GET: PurchasePaymentReturn
        public async Task<ActionResult> ReturnPurchasePendingAmount()
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
                var list = await _supplierReturnPaymentRepository.GetBySupplierReturnInvoiceId(id.Value);

                double remainingAmount = list.Sum(item => item.RemainingBalance);
                if (remainingAmount == 0) return RedirectToAction("AllPurchasesPendingPayment");

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
        public async Task<ActionResult> ReturnAmount(int id, float previousRemainingAmount, float paymentAmount)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var returnAmountDto = new PurchaseReturnAmount
                {
                    InvoiceId = id,
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
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}