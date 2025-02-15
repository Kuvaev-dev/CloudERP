using System;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers.Purchase.Cart
{
    [RoutePrefix("api/purchase-return")]
    public class PurchaseReturnApiController : ApiController
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
        [Route("invoice/{invoiceID}")]
        public async Task<IHttpActionResult> GetPurchaseByInvoice(string invoiceID)
        {
            try
            {
                var invoice = await _supplierInvoiceRepository.GetByInvoiceNoAsync(invoiceID);
                if (invoice == null)
                    return NotFound();

                var invoiceDetails = _supplierReturnInvoiceDetailRepository.GetInvoiceDetails(invoiceID);
                return Ok(new { invoice, invoiceDetails });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("return")]
        public async Task<IHttpActionResult> ProcessPurchaseReturn(PurchaseReturnConfirm returnConfirmDto)
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
                return InternalServerError(ex);
            }
        }
    }
}