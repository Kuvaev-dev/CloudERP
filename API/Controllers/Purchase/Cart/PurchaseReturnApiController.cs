using Domain.Models.FinancialModels;
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
        public async Task<ActionResult<object>> GetPurchaseByInvoice(string invoiceID)
        {
            try
            {
                var invoice = await _supplierInvoiceRepository.GetByInvoiceNoAsync(invoiceID);
                if (invoice == null) return NotFound();

                var invoiceDetails = _supplierReturnInvoiceDetailRepository.GetInvoiceDetails(invoiceID);
                if (invoiceDetails == null) return NotFound();

                return Ok(new { invoice, invoiceDetails });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<object>> ProcessPurchaseReturn(PurchaseReturnConfirm returnConfirmDto)
        {
            try
            {
                var result = await _purchaseReturnService.ProcessReturnAsync(
                    returnConfirmDto,
                    returnConfirmDto.BranchID,
                    returnConfirmDto.CompanyID,
                    returnConfirmDto.UserID);

                if (result.IsSuccess)
                {
                    return Ok(new { invoiceNo = result.InvoiceNo, message = result.Message });
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}