using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Purchase.Cart
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class PurchaseReturnApiController : ControllerBase
    {
        private readonly ISupplierInvoiceRepository _supplierInvoiceRepository;
        private readonly ISupplierReturnInvoiceDetailRepository _supplierReturnInvoiceDetailRepository;
        private readonly IPurchaseReturnService _purchaseReturnService;

        public PurchaseReturnApiController(
            ISupplierInvoiceRepository supplierInvoiceRepository,
            IPurchaseReturnService purchaseReturnService,
            ISupplierReturnInvoiceDetailRepository supplierReturnInvoiceDetailRepository)
        {
            _supplierInvoiceRepository = supplierInvoiceRepository;
            _purchaseReturnService = purchaseReturnService;
            _supplierReturnInvoiceDetailRepository = supplierReturnInvoiceDetailRepository;
        }

        [HttpGet]
        public async Task<ActionResult<FindPuchaseResponse>> FindPurchase(string invoiceID)
        {
            try
            {
                var invoice = await _supplierInvoiceRepository.GetByInvoiceNoAsync(invoiceID);
                if (invoice == null) return NotFound();

                var invoiceDetails = _supplierReturnInvoiceDetailRepository.GetInvoiceDetails(invoiceID);
                if (invoiceDetails == null) return NotFound();

                return Ok(new FindPuchaseResponse { Invoice = invoice, InvoiceDetails = invoiceDetails });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<PurchaseReturnConfirmResult>> ProcessPurchaseReturn(PurchaseReturnConfirm returnConfirmDto)
        {
            try
            {
                var result = await _purchaseReturnService.ProcessReturnAsync(returnConfirmDto);
                if (result.IsSuccess)
                {
                    return Ok(new PurchaseReturnConfirmResult { InvoiceNo = result.InvoiceNo, IsSuccess = true, Message = result.Message });
                }

                return Ok(new PurchaseReturnConfirmResult { InvoiceNo = string.Empty, IsSuccess = false, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Ok(new PurchaseReturnConfirmResult { InvoiceNo = string.Empty, IsSuccess = false, Message = "Unexpected error: " + ex.Message });
            }
        }
    }
}