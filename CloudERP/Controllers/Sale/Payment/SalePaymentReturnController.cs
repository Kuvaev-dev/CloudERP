using CloudERP.Helpers;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SalePaymentReturnController : Controller
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerReturnPaymentRepository _customerReturnPaymentRepository;
        private readonly ISalePaymentReturnService _salePaymentReturnService;
        private readonly SessionHelper _sessionHelper;

        public SalePaymentReturnController(
            ISaleRepository saleRepository,
            ICustomerReturnPaymentRepository customerReturnPaymentRepository,
            ISalePaymentReturnService salePaymentReturnService,
            SessionHelper sessionHelper)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(ISaleRepository));
            _customerReturnPaymentRepository = customerReturnPaymentRepository ?? throw new ArgumentNullException(nameof(ICustomerReturnPaymentRepository));
            _salePaymentReturnService = salePaymentReturnService ?? throw new ArgumentNullException(nameof(ISalePaymentReturnService));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: SalePaymentReturn
        public async Task<ActionResult> ReturnSalePendingAmount()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _saleRepository.GetReturnSaleAmountPending(_sessionHelper.CompanyID, _sessionHelper.BranchID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AllReturnSalesPendingAmount()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _saleRepository.GetReturnSaleAmountPending(_sessionHelper.CompanyID, _sessionHelper.BranchID));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> ReturnAmount(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var list = await _customerReturnPaymentRepository.GetListByReturnInvoiceIdAsync(id.Value);

                double remainingAmount = list.Sum(item => item.RemainingBalance);
                if (remainingAmount == 0) return RedirectToAction("AllReturnSalesPendingAmount");

                ViewBag.PreviousRemainingAmount = remainingAmount;
                ViewBag.InvoiceID = id;

                return View(list);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                var paymentReturnDto = new SalePaymentReturn
                {
                    InvoiceId = id,
                    PreviousRemainingAmount = previousRemainingAmount,
                    PaymentAmount = paymentAmount
                };

                var result = await _salePaymentReturnService.ProcessReturnAmountAsync(
                    paymentReturnDto,
                    _sessionHelper.BranchID,
                    _sessionHelper.CompanyID,
                    _sessionHelper.UserID);

                Session["SaleMessage"] = result.Message;

                return RedirectToAction("PurchasePaymentReturn", new { id });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}