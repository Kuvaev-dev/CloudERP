using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Sale.Cart
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SaleCartApiController : ControllerBase
    {
        private readonly ISaleCartDetailRepository _saleCartDetailRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISaleCartService _saleCartService;

        public SaleCartApiController(
            ISaleCartDetailRepository saleCartDetailRepository,
            IStockRepository stockRepository,
            ISaleCartService saleCartService)
        {
            _saleCartDetailRepository = saleCartDetailRepository ?? throw new ArgumentNullException(nameof(saleCartDetailRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
            _saleCartService = saleCartService ?? throw new ArgumentNullException(nameof(saleCartService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SaleCartDetail>>> GetSaleCartDetails(int branchId, int companyId, int userId)
        {
            try
            {
                var details = await _saleCartDetailRepository.GetByDefaultSettingAsync(branchId, companyId, userId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetProductDetails(int id)
        {
            try
            {
                var product = await _stockRepository.GetByIdAsync(id);
                if (product == null) return NotFound();
                return Ok(new { product?.SaleUnitPrice });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddItem(SaleCartDetail item)
        {
            try
            {
                await _saleCartDetailRepository.AddAsync(item);
                if (item == null) return BadRequest("Invalid data.");
                return Ok(new { message = "Item added successfully" });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<string>> DeleteItem(int id)
        {
            try
            {
                await _saleCartDetailRepository.DeleteAsync(id);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> CancelSale(int branchId, int companyId, int userId)
        {
            try
            {
                var details = await _saleCartDetailRepository.GetByDefaultSettingAsync(branchId, companyId, userId);
                if (details == null) return BadRequest();
                await _saleCartDetailRepository.DeleteListAsync(details);
                return Ok(new { message = "Sale canceled" });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult> ConfirmSale(SaleConfirm saleConfirmDto)
        {
            try
            {
                var result = await _saleCartService.ConfirmSaleAsync(saleConfirmDto);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { error = result.ErrorMessage });
                }

                return Ok(new Dictionary<string, int> { { "invoiceId", result.Value } });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}