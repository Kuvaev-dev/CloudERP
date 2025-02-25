using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Purchase.Payment
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class PurchasePaymentReturnApiController : ControllerBase
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
        public async Task<ActionResult<List<PurchaseInfo>>> GetReturnPurchasePendingAmount(int companyId, int branchId)
        {
            var purchases = await _purchaseRepository.GetReturnPurchasesPaymentPending(companyId, branchId);
            if (purchases == null) return NotFound();
            return Ok(purchases);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierReturnPayment>>> GetSupplierReturnPayments(int id)
        {
            var payments = await _supplierReturnPaymentRepository.GetBySupplierReturnInvoiceId(id);
            if (payments == null) return NotFound();
            return Ok(payments);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ProcessReturnAmount(PurchaseReturn returnAmountDto, int companyId, int branchId, int userId)
        {
            string message = await _purchasePaymentReturnService.ProcessReturnPaymentAsync(returnAmountDto, branchId, companyId, userId);
            return Ok(message);
        }
    }
}