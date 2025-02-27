using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Facades;

namespace API.Controllers.Sale.Payment
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SalePaymentApiController : ControllerBase
    {
        private readonly SalePaymentFacade _salePaymentFacade;

        public SalePaymentApiController(SalePaymentFacade salePaymentFacade)
        {
            _salePaymentFacade = salePaymentFacade ?? throw new ArgumentNullException(nameof(salePaymentFacade));
        }

        [HttpGet]
        public async Task<ActionResult<List<SaleInfo>>> GetRemainingPaymentList(int companyID, int branchID)
        {
            var list = await _salePaymentFacade.SaleRepository.RemainingPaymentList(companyID, branchID);
            if (list.Count == 0) return NotFound();
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<List<SaleInfo>>> GetPaidHistory(int id)
        {
            var history = await _salePaymentFacade.SalePaymentService.GetSalePaymentHistoryAsync(id);
            if (history.Count == 0) return NotFound();
            return Ok(history);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerReturnInvoice>>> GetReturnSaleDetails(int invoiceID)
        {
            var returnDetails = await _salePaymentFacade.CustomerReturnInvoiceRepository.GetReturnDetails(invoiceID);
            if (returnDetails == null) return NotFound();
            return Ok(returnDetails);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ProcessPayment(SaleAmount paymentDto)
        {
            string message = await _salePaymentFacade.SalePaymentService.ProcessPaymentAsync(
                paymentDto.CompanyID,
                paymentDto.BranchID,
                paymentDto.UserID,
                paymentDto);

            if (message == Localization.CloudERP.Messages.PurchasePaymentRemainingAmountError)
                return BadRequest(message);

            return Ok(message);
        }

        [HttpGet]
        public async Task<ActionResult<List<SaleInfo>>> GetCustomSalesHistory(int companyID, int branchID, DateTime fromDate, DateTime toDate)
        {
            var list = await _salePaymentFacade.SaleRepository.CustomSalesList(companyID, branchID, fromDate, toDate);
            if (list == null) return NotFound();
            return Ok(list);
        }

        [HttpGet]
        public async Task<ActionResult<PurchaseItemDetailDto>> GetSaleItemDetail(int id)
        {
            var saleDetail = await _salePaymentFacade.SaleService.GetSaleItemDetailAsync(id);
            if (saleDetail == null) return NotFound();
            return Ok(saleDetail);
        }

        [HttpGet]
        public async Task<ActionResult<SupplierInvoiceDetail>> GetSaleInvoice(int id)
        {
            var invoiceDetails = await _salePaymentFacade.CustomerInvoiceDetailRepository.GetListByIdAsync(id);
            if (invoiceDetails == null) return NotFound();
            return Ok(invoiceDetails);
        }

        [HttpGet]
        public async Task<ActionResult<double>> GetRemainingAmount(int id)
        {
            double? totalInvoiceAmount = await _salePaymentFacade.SalePaymentService.GetTotalAmountByIdAsync(id);
            double totalPaidAmount = await _salePaymentFacade.SalePaymentService.GetTotalPaidAmountByIdAsync(id);
            double remainingAmount = totalInvoiceAmount.Value - totalPaidAmount;
            return Ok(remainingAmount);
        }

        [HttpGet]
        public async Task<ActionResult<double>> GetTotalAmount(int id)
        {
            double? totalInvoiceAmount = await _salePaymentFacade.SalePaymentService.GetTotalAmountByIdAsync(id);
            return Ok(totalInvoiceAmount);
        }
    }
}