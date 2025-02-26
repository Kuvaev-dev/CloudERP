using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Sale.Payment
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SalePaymentReturnApiController : ControllerBase
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
        public async Task<ActionResult<List<SaleInfo>>> GetReturnSalePendingAmount(int companyId, int branchId)
        {
            var result = await _saleRepository.GetReturnSaleAmountPending(companyId, branchId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerReturnPayment>>> GetCustomerReturnPayments(int id)
        {
            var payments = await _customerReturnPaymentRepository.GetByCustomerReturnInvoiceId(id);
            if (payments == null) return NotFound();
            return Ok(payments);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ProcessReturnAmount(SaleReturn paymentReturnDto, int branchId, int companyId, int userId)
        {
            string message = await _salePaymentReturnService.ProcessReturnAmountAsync(paymentReturnDto, branchId, companyId, userId);
            return Ok(message);
        }
    }
}