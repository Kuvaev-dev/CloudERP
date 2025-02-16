using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/sale-return")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SaleReturnApiController : ApiController
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
        [Route("find-sale/{invoiceID}")]
        public async Task<IHttpActionResult> FindSale(string invoiceID)
        {
            var invoiceDetails = _customerReturnInvoiceDetailRepository.GetInvoiceDetails(invoiceID);
            var invoice = await _customerInvoiceRepository.GetByInvoiceNoAsync(invoiceID);

            return Ok(new { InvoiceDetails = invoiceDetails, Invoice = invoice });
        }

        [HttpPost]
        [Route("return-confirm")]
        public async Task<IHttpActionResult> ReturnConfirm(SaleReturnConfirm returnConfirmDto)
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