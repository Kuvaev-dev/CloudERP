using Domain.Facades;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.Models.Purchase;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Purchase.Payment
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class PurchasePaymentApiController : ControllerBase
    {
        private readonly PurchasePaymentFacade _purchasePaymentFacade;

        public PurchasePaymentApiController(PurchasePaymentFacade purchasePaymentFacade)
        {
            _purchasePaymentFacade = purchasePaymentFacade ?? throw new ArgumentNullException(nameof(purchasePaymentFacade));
        }

        [HttpGet]
        public async Task<ActionResult<List<PurchasePaymentModel>>> GetRemainingPaymentList(int companyId, int branchId)
        {
            var list = await _purchasePaymentFacade.PurchaseRepository.RemainingPaymentList(companyId, branchId);
            if (list.Count == 0) return NotFound();
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<List<PurchasePaymentModel>>> GetPaidHistory(int id)
        {
            var list = await _purchasePaymentFacade.PurchasePaymentService.GetPurchasePaymentHistoryAsync(id);
            if (list.Count == 0) return NotFound();
            return Ok(list);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ProcessPayment(PurchasePayment paymentDto)
        {
            string message = await _purchasePaymentFacade.PurchasePaymentService.ProcessPaymentAsync(
                paymentDto.CompanyID,
                paymentDto.BranchID,
                paymentDto.UserID,
                paymentDto);

            if (message == Localization.CloudERP.Messages.Messages.PurchasePaymentRemainingAmountError)
                return BadRequest(message);

            return Ok(message);
        }

        [HttpGet]
        public async Task<ActionResult<List<PurchasePaymentModel>>> GetCustomPurchasesHistory(int companyId, int branchId, DateTime fromDate, DateTime toDate)
        {
            var list = await _purchasePaymentFacade.PurchaseRepository.CustomPurchasesList(companyId, branchId, fromDate, toDate);
            if (list.Count == 0) return NotFound();
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<PurchaseItemDetailDto>> GetPurchaseItemDetail(int id)
        {
            var purchaseDetail = await _purchasePaymentFacade.PurchaseService.GetPurchaseItemDetailAsync(id);
            if (purchaseDetail == null) return NotFound();
            return Ok(purchaseDetail);
        }

        [HttpGet]
        public async Task<ActionResult<SupplierInvoiceDetail>> GetPurchaseInvoice(int id)
        {
            var invoiceDetails = await _purchasePaymentFacade.SupplierInvoiceDetailRepository.GetListByIdAsync(id);
            if (invoiceDetails == null) return NotFound();
            return Ok(invoiceDetails);
        }
    }
}