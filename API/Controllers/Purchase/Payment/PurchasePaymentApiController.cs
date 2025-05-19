using Domain.Facades;
using Domain.Models;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Purchase.Payment
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class PurchasePaymentApiController : ControllerBase
    {
        private readonly PurchasePaymentFacade _purchasePaymentFacade;
        private readonly IPurchasePaymentService _purchasePaymentService;

        public PurchasePaymentApiController(
            PurchasePaymentFacade purchasePaymentFacade,
            IPurchasePaymentService purchasePaymentService)
        {
            _purchasePaymentFacade = purchasePaymentFacade ?? throw new ArgumentNullException(nameof(purchasePaymentFacade));
            _purchasePaymentService = purchasePaymentService ?? throw new ArgumentNullException(nameof(purchasePaymentService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseInfo>>> GetRemainingPaymentList(int companyId, int branchId)
        {
            var list = await _purchasePaymentFacade.PurchaseRepository.RemainingPaymentList(companyId, branchId);
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseInfo>>> GetPaidHistory(int id)
        {
            var list = await _purchasePaymentService.GetPurchasePaymentHistoryAsync(id);
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierReturnInvoice>>> GetReturnPurchaseDetails(int invoiceID)
        {
            var returnDetails = await _purchasePaymentFacade.SupplierReturnInvoiceRepository.GetReturnDetails(invoiceID);
            return Ok(returnDetails);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ProcessPayment(PurchaseAmount paymentDto)
        {
            string message = await _purchasePaymentService.ProcessPaymentAsync(
                paymentDto.CompanyID,
                paymentDto.BranchID,
                paymentDto.UserID,
                paymentDto);

            if (message == Localization.CloudERP.Messages.Messages.PurchasePaymentRemainingAmountError)
                return BadRequest(message);

            return Ok(message);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseInfo>>> GetCustomPurchasesHistory(int companyId, int branchId, DateTime fromDate, DateTime toDate)
        {
            var list = await _purchasePaymentFacade.PurchaseRepository.CustomPurchasesList(companyId, branchId, fromDate, toDate);
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<PurchaseItemDetailDto>> GetPurchaseItemDetail(int id)
        {
            var purchaseDetail = await _purchasePaymentFacade.PurchaseService.GetPurchaseItemDetailAsync(id);
            return Ok(purchaseDetail);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierInvoiceDetail>>> GetPurchaseInvoice(int id)
        {
            var invoiceDetails = await _purchasePaymentFacade.SupplierInvoiceDetailRepository.GetListByIdAsync(id);
            return Ok(invoiceDetails);
        }

        [HttpGet]
        public async Task<ActionResult<double>> GetRemainingAmount(int id)
        {
            double? totalInvoiceAmount = await _purchasePaymentService.GetTotalAmountByIdAsync(id) ?? 0;
            double totalPaidAmount = await _purchasePaymentService.GetTotalPaidAmountByIdAsync(id);
            double remainingAmount = totalInvoiceAmount.Value - totalPaidAmount;
            return Ok(remainingAmount);
        }

        [HttpGet]
        public async Task<ActionResult<double>> GetTotalAmount(int id)
        {
            double? totalInvoiceAmount = await _purchasePaymentService.GetTotalAmountByIdAsync(id);
            return Ok(totalInvoiceAmount);
        }
    }
}