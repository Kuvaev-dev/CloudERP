using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Sale.Payment
{
    [ApiController]
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
        public async Task<ActionResult<List<SalePaymentModel>>> GetReturnSalePendingAmount(int companyId, int branchId)
        {
            var result = await _saleRepository.GetReturnSaleAmountPending(companyId, branchId);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetReturnAmount(int invoiceID)
        {
            var list = await _customerReturnPaymentRepository.GetListByReturnInvoiceIdAsync(invoiceID);
            if (list == null) return NotFound();

            double remainingAmount = list.Sum(item => item.RemainingBalance);

            return Ok(new { RemainingAmount = remainingAmount, Payments = list });
        }

        [HttpPost]
        public async Task<ActionResult<(bool IsSuccess, string Message, IEnumerable<CustomerReturnPayment> Items, double RemainingAmount)>> 
            ProcessReturnAmount(SalePaymentReturn paymentReturnDto, int branchId, int companyId, int userId)
        {
            var result = await _salePaymentReturnService.ProcessReturnAmountAsync(paymentReturnDto, branchId, companyId, userId);
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result);
        }
    }
}