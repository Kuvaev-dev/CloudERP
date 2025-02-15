using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/salepayment")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SalePaymentApiController : ApiController
    {
        private readonly ISalePaymentService _salePaymentService;
        private readonly ISaleRepository _saleRepository;

        public SalePaymentApiController(ISalePaymentService salePaymentService, ISaleRepository saleRepository)
        {
            _salePaymentService = salePaymentService ?? throw new ArgumentNullException(nameof(salePaymentService));
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
        }

        [HttpGet]
        [Route("remainingPaymentList/{companyID}/{branchID}")]
        public async Task<IHttpActionResult> GetRemainingPaymentList(int companyID, int branchID)
        {
            var list = await _saleRepository.RemainingPaymentList(companyID, branchID);
            return Ok(list);
        }

        [HttpGet]
        [Route("salePaymentHistory/{invoiceID}")]
        public async Task<IHttpActionResult> GetSalePaymentHistory(int invoiceID)
        {
            var history = await _salePaymentService.GetSalePaymentHistoryAsync(invoiceID);
            return Ok(history);
        }

        [HttpPost]
        [Route("processPayment")]
        public async Task<IHttpActionResult> ProcessPayment(SalePayment paymentDto, int branchId, int companyId, int userId)
        {
            var message = await _salePaymentService.ProcessPaymentAsync(paymentDto, branchId, companyId, userId);
            return Ok(new { Message = message });
        }

        [HttpGet]
        [Route("customSalesHistory/{companyID}/{branchID}/{fromDate}/{toDate}")]
        public async Task<IHttpActionResult> GetCustomSalesHistory(int companyID, int branchID, DateTime fromDate, DateTime toDate)
        {
            var list = await _saleRepository.CustomSalesList(companyID, branchID, fromDate, toDate);
            return Ok(list);
        }
    }
}