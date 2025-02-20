using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Sale.Cart
{
    [ApiController]
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
        public async Task<ActionResult<object>> FindSale(string invoiceID)
        {
            var invoiceDetails = _customerReturnInvoiceDetailRepository.GetInvoiceDetails(invoiceID);
            if (invoiceDetails == null) return NotFound();

            var invoice = await _customerInvoiceRepository.GetByInvoiceNoAsync(invoiceID);
            if (invoice == null) return NotFound();

            return Ok(new { InvoiceDetails = invoiceDetails, Invoice = invoice });
        }

        [HttpPost]
        public async Task<ActionResult<ReturnConfirmResult>> ReturnConfirm(SaleReturnConfirm returnConfirmDto)
        {
            var result = await _saleReturnService.ProcessReturnConfirmAsync(
                returnConfirmDto,
                returnConfirmDto.BranchID,
                returnConfirmDto.CompanyID,
                returnConfirmDto.UserID);

            return Ok(result);
        }
    }
}