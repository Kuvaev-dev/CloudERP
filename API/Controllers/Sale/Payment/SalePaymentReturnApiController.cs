using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/salepaymentreturn")]
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
        [Route("returnSalePendingAmount/{companyID}/{branchID}")]
        public async Task<IHttpActionResult> GetReturnSalePendingAmount(int companyID, int branchID)
        {
            var result = await _saleRepository.GetReturnSaleAmountPending(companyID, branchID);
            return Ok(result);
        }

        [HttpGet]
        [Route("returnAmount/{invoiceID}")]
        public async Task<IHttpActionResult> GetReturnAmount(int invoiceID)
        {
            var list = await _customerReturnPaymentRepository.GetListByReturnInvoiceIdAsync(invoiceID);
            double remainingAmount = list.Sum(item => item.RemainingBalance);
            return Ok(new { RemainingAmount = remainingAmount, Payments = list });
        }

        [HttpPost]
        [Route("processReturnAmount")]
        public async Task<IHttpActionResult> ProcessReturnAmount(SalePaymentReturn paymentReturnDto, int branchId, int companyId, int userId)
        {
            var result = await _salePaymentReturnService.ProcessReturnAmountAsync(paymentReturnDto, branchId, companyId, userId);
            return Ok(result);
        }
    }
}