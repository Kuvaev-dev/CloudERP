using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/sale-payment-return")]
    public class SalePaymentReturnApiController : ApiController
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ICustomerReturnPaymentRepository _customerReturnPaymentRepository;
        private readonly ISalePaymentReturnService _salePaymentReturnService;

        public SalePaymentReturnApiController(
            ISaleRepository saleRepository,
            ICustomerReturnPaymentRepository customerReturnPaymentRepository,
            ISalePaymentReturnService salePaymentReturnService)
        {
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(saleRepository));
            _customerReturnPaymentRepository = customerReturnPaymentRepository ?? throw new ArgumentNullException(nameof(customerReturnPaymentRepository));
            _salePaymentReturnService = salePaymentReturnService ?? throw new ArgumentNullException(nameof(salePaymentReturnService));
        }

        [HttpGet]
        [Route("return-sale-pending-amount?companyId={companyId:int}&branchId={branchId:int}")]
        public async Task<IHttpActionResult> GetReturnSalePendingAmount([FromUri] int companyId, [FromUri] int branchId)
        {
            var result = await _saleRepository.GetReturnSaleAmountPending(companyId, branchId);
            return Ok(result);
        }

        [HttpGet]
        [Route("return-amount?invoiceID={invoiceID:int}")]
        public async Task<IHttpActionResult> GetReturnAmount([FromUri] int invoiceID)
        {
            var list = await _customerReturnPaymentRepository.GetListByReturnInvoiceIdAsync(invoiceID);
            double remainingAmount = list.Sum(item => item.RemainingBalance);
            return Ok(new { RemainingAmount = remainingAmount, Payments = list });
        }

        [HttpPost]
        [Route("process-return-amount?companyId={companyId:int}&branchId={branchId:int}&userId={userId:int}")]
        public async Task<IHttpActionResult> ProcessReturnAmount(SalePaymentReturn paymentReturnDto, [FromUri] int branchId, [FromUri] int companyId, [FromUri] int userId)
        {
            var result = await _salePaymentReturnService.ProcessReturnAmountAsync(paymentReturnDto, branchId, companyId, userId);
            return Ok(result);
        }
    }
}