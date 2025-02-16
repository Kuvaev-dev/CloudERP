using System;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/purchase-payment-return")]
    public class PurchasePaymentReturnApiController : ApiController
    {
        private readonly IPurchasePaymentReturnService _purchasePaymentReturnService;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly ISupplierReturnPaymentRepository _supplierReturnPaymentRepository;

        public PurchasePaymentReturnApiController(
            IPurchasePaymentReturnService purchasePaymentReturnService,
            IPurchaseRepository purchaseRepository,
            ISupplierReturnPaymentRepository supplierReturnPaymentRepository)
        {
            _purchasePaymentReturnService = purchasePaymentReturnService ?? throw new ArgumentNullException(nameof(purchasePaymentReturnService));
            _purchaseRepository = purchaseRepository ?? throw new ArgumentNullException(nameof(purchaseRepository));
            _supplierReturnPaymentRepository = supplierReturnPaymentRepository ?? throw new ArgumentNullException(nameof(supplierReturnPaymentRepository));
        }

        [HttpGet]
        [Route("return-purchase-pending-amount?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetReturnPurchasePendingAmount([FromUri] int companyId, [FromUri] int branchId)
        {
            var purchases = await _purchaseRepository.GetReturnPurchasesPaymentPending(companyId, branchId);
            return Ok(purchases);
        }

        [HttpGet]
        [Route("supplier-return-payments/{id}")]
        public async Task<IHttpActionResult> GetSupplierReturnPayments(int id)
        {
            var payments = await _supplierReturnPaymentRepository.GetBySupplierReturnInvoiceId(id);
            return Ok(payments);
        }

        [HttpPost]
        [Route("process-return-payment?companyId={companyId:int}&branchId={branchId:int}&userId={userId:int}")]
        public async Task<IHttpActionResult> ProcessReturnPayment(PurchaseReturnAmount returnAmountDto, [FromUri] int companyId, [FromUri] int branchId, [FromUri] int userId)
        {
            string message = await _purchasePaymentReturnService.ProcessReturnPaymentAsync(returnAmountDto, branchId, companyId, userId);
            return Ok(message);
        }
    }
}