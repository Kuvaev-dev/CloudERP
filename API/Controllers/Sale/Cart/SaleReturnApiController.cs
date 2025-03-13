using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Sale.Cart
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SaleReturnApiController : ControllerBase
    {
        private readonly ICustomerInvoiceRepository _customerInvoiceRepository;
        private readonly ICustomerReturnInvoiceDetailRepository _customerReturnInvoiceDetailRepository;
        private readonly ISaleReturnService _saleReturnService;

        public SaleReturnApiController(
            ICustomerInvoiceRepository customerInvoiceRepository,
            ICustomerReturnInvoiceDetailRepository customerReturnInvoiceDetailRepository,
            ISaleReturnService saleReturnService)
        {
            _customerInvoiceRepository = customerInvoiceRepository ?? throw new ArgumentNullException(nameof(customerInvoiceRepository));
            _customerReturnInvoiceDetailRepository = customerReturnInvoiceDetailRepository ?? throw new ArgumentNullException(nameof(customerReturnInvoiceDetailRepository));
            _saleReturnService = saleReturnService ?? throw new ArgumentNullException(nameof(saleReturnService));
        }

        [HttpGet]
        public async Task<ActionResult<FindSaleResponse>> FindSale(string invoiceID)
        {
            try
            {
                var invoice = await _customerInvoiceRepository.GetByInvoiceNoAsync(invoiceID);
                if (invoice == null) return NotFound();

                var invoiceDetails = _customerReturnInvoiceDetailRepository.GetInvoiceDetails(invoiceID);
                if (invoiceDetails == null) return NotFound();

                return Ok(new FindSaleResponse { Invoice = invoice, InvoiceDetails = invoiceDetails });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<SaleReturnConfirmResult>> ProcessSaleReturn(SaleReturnConfirm returnConfirmDto)
        {
            try
            {
                var result = await _saleReturnService.ProcessReturnConfirmAsync(returnConfirmDto);

                if (result.IsSuccess)
                {
                    return Ok(new SaleReturnConfirmResult { InvoiceNo = result.InvoiceNo, IsSuccess = true, Message = result.Message });
                }

                return Ok(new SaleReturnConfirmResult { InvoiceNo = string.Empty, IsSuccess = false, Message = result.Message });
            }
            catch (Exception ex)
            {
                return Ok(new SaleReturnConfirmResult { InvoiceNo = string.Empty, IsSuccess = false, Message = "Unexpected error: " + ex.Message });
            }
        }
    }
}