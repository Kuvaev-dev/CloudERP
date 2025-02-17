using Domain.Facades;
using Domain.Models.FinancialModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CloudERP.ApiControllers
{
    [RoutePrefix("api/purchase-payment")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class PurchasePaymentApiController : ApiController
    {
        private readonly PurchasePaymentFacade _purchasePaymentFacade;

        public PurchasePaymentApiController(PurchasePaymentFacade purchasePaymentFacade)
        {
            _purchasePaymentFacade = purchasePaymentFacade ?? throw new ArgumentNullException(nameof(purchasePaymentFacade));
        }

        [HttpGet, Route("remaining-payment-list/{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetRemainingPaymentList(int companyId, int branchId)
        {
            var list = await _purchasePaymentFacade.PurchaseRepository.RemainingPaymentList(companyId, branchId);
            return Ok(list);
        }

        [HttpGet, Route("paid-history/{id:int}")]
        public async Task<IHttpActionResult> GetPaidHistory(int id)
        {
            var list = await _purchasePaymentFacade.PurchasePaymentService.GetPurchasePaymentHistoryAsync(id);
            return Ok(list);
        }

        [HttpPost, Route("process-payment")]
        public async Task<IHttpActionResult> ProcessPayment(PurchasePayment paymentDto)
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

        [HttpGet, Route("custom-purchases-history/{companyId:int}/{branchId:int}/{fromDate:DateTime}/{toDate:DateTime}")]
        public async Task<IHttpActionResult> GetCustomPurchasesHistory(int companyId, int branchId, DateTime fromDate, DateTime toDate)
        {
            var list = await _purchasePaymentFacade.PurchaseRepository.CustomPurchasesList(companyId, branchId, fromDate, toDate);
            return Ok(list);
        }

        [HttpGet, Route("purchase-item-detail/{id:int}")]
        public async Task<IHttpActionResult> GetPurchaseItemDetail(int id)
        {
            var purchaseDetail = await _purchasePaymentFacade.PurchaseService.GetPurchaseItemDetailAsync(id);
            return Ok(purchaseDetail);
        }

        [HttpGet, Route("purchase-invoice/{id:int}")]
        public async Task<IHttpActionResult> GetPurchaseInvoice(int id)
        {
            var invoiceDetails = await _purchasePaymentFacade.SupplierInvoiceDetailRepository.GetListByIdAsync(id);
            if (invoiceDetails?.Any() != true)
                return NotFound();

            return Ok(invoiceDetails);
        }
    }
}