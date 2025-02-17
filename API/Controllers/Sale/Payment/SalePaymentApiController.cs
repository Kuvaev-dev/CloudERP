using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
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
        [Route("remaining-payment-list/{companyID:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetRemainingPaymentList(int companyID, int branchID)
        {
            var list = await _saleRepository.RemainingPaymentList(companyID, branchID);
            return Ok(list);
        }

        [HttpGet]
        [Route("sale-payment-history/{invoiceID:int}")]
        public async Task<IHttpActionResult> GetSalePaymentHistory(int invoiceID)
        {
            var history = await _salePaymentService.GetSalePaymentHistoryAsync(invoiceID);
            return Ok(history);
        }

        [HttpGet]
        [Route("get-return-sale-details/{invoiceID:int}")]
        public async Task<IHttpActionResult> GetReturnSaleDetails(int invoiceID)
        {
            var returnDetails = await _customerReturnInvoiceRepository.GetListByIdAsync(invoiceID);
            return Ok(returnDetails);
        }

        [HttpPost]
        [Route("process-payment/{branchId:int}/{companyId:int}/{userId:int}")]
        public async Task<IHttpActionResult> ProcessPayment(SalePayment paymentDto, int branchId, int companyId, int userId)
        {
            var message = await _salePaymentService.ProcessPaymentAsync(paymentDto, branchId, companyId, userId);
            return Ok(new { Message = message });
        }

        [HttpGet]
        [Route("custom-sales-history/{companyID:int}/{branchID:int}/{fromDate:DateTime}/{toDate:DateTime}")]
        public async Task<IHttpActionResult> GetCustomSalesHistory(int companyID, int branchID, DateTime fromDate, DateTime toDate)
        {
            var list = await _saleRepository.CustomSalesList(companyID, branchID, fromDate, toDate);
            return Ok(list);
        }
    }
}