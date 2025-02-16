using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Facades;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/sale-payment")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SalePaymentApiController : ApiController
    {
        private readonly ISalePaymentService _salePaymentService;
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerReturnInvoiceRepository _customerReturnInvoiceRepository;

        public SalePaymentApiController(
            ISalePaymentService salePaymentService, 
            ISaleRepository saleRepository,
            ICustomerReturnInvoiceRepository customerReturnInvoiceRepository)
        {
            _salePaymentService = salePaymentService ?? throw new ArgumentNullException(nameof(salePaymentService));
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
            _customerReturnInvoiceRepository = customerReturnInvoiceRepository;
        }

        [HttpGet]
        [Route("remaining-payment-list?companyID={companyID:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetRemainingPaymentList([FromUri] int companyID, [FromUri] int branchID)
        {
            var list = await _saleRepository.RemainingPaymentList(companyID, branchID);
            return Ok(list);
        }

        [HttpGet]
        [Route("sale-payment-history?invoiceID={invoiceID:int}")]
        public async Task<IHttpActionResult> GetSalePaymentHistory([FromUri] int invoiceID)
        {
            var history = await _salePaymentService.GetSalePaymentHistoryAsync(invoiceID);
            return Ok(history);
        }

        [HttpGet]
        [Route("get-return-sale-details?invoiceID={invoiceID:int}")]
        public async Task<IHttpActionResult> GetReturnSaleDetails([FromUri] int invoiceID)
        {
            var returnDetails = await _customerReturnInvoiceRepository.GetListByIdAsync(invoiceID);
            return Ok(returnDetails);
        }

        [HttpPost]
        [Route("process-payment?branchId={branchId:int}&companyId={companyId:int}&userId={userId:int}")]
        public async Task<IHttpActionResult> ProcessPayment(SalePayment paymentDto, [FromUri] int branchId, [FromUri] int companyId, [FromUri] int userId)
        {
            var message = await _salePaymentService.ProcessPaymentAsync(paymentDto, branchId, companyId, userId);
            return Ok(new { Message = message });
        }

        [HttpGet]
        [Route("custom-sales-history?companyID={companyID:int}&branchID={branchID:int}&fromDate={fromDate:DateTime}&toDate={toDate:DateTime}")]
        public async Task<IHttpActionResult> GetCustomSalesHistory([FromUri] int companyID, [FromUri] int branchID, [FromUri] DateTime fromDate, [FromUri] DateTime toDate)
        {
            var list = await _saleRepository.CustomSalesList(companyID, branchID, fromDate, toDate);
            return Ok(list);
        }
    }
}