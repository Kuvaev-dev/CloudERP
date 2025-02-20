using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Sale.Payment
{
    [ApiController]
    public class SalePaymentApiController : ControllerBase
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
        public async Task<ActionResult<List<SalePaymentModel>>> GetRemainingPaymentList(int companyID, int branchID)
        {
            var list = await _saleRepository.RemainingPaymentList(companyID, branchID);
            if (list == null) return NotFound();
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<List<SalePaymentModel>>> GetSalePaymentHistory(int invoiceID)
        {
            var history = await _salePaymentService.GetSalePaymentHistoryAsync(invoiceID);
            if (history == null) return NotFound();
            return Ok(history);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerReturnInvoice>>> GetReturnSaleDetails(int invoiceID)
        {
            var returnDetails = await _customerReturnInvoiceRepository.GetListByIdAsync(invoiceID);
            if (returnDetails == null) return NotFound();
            return Ok(returnDetails);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ProcessPayment(SalePayment paymentDto, int branchId, int companyId, int userId)
        {
            var message = await _salePaymentService.ProcessPaymentAsync(paymentDto, branchId, companyId, userId);
            return Ok(new { Message = message });
        }

        [HttpGet]
        public async Task<ActionResult<List<SalePaymentModel>>> GetCustomSalesHistory(int companyID, int branchID, DateTime fromDate, DateTime toDate)
        {
            var list = await _saleRepository.CustomSalesList(companyID, branchID, fromDate, toDate);
            if (list == null) return NotFound();
            return Ok(list);
        }
    }
}