using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Implementations;

namespace API.Controllers.Purchase.Cart
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class PurchaseCartApiController : ControllerBase
    {
        private readonly IPurchaseCartDetailRepository _purchaseCartDetailRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IPurchaseCartService _purchaseCartService;

        public PurchaseCartApiController(
            IPurchaseCartDetailRepository purchaseCartDetailRepository,
            IStockRepository stockRepository,
            IPurchaseCartService purchaseCartService,
            ISupplierRepository supplierRepository)
        {
            _purchaseCartDetailRepository = purchaseCartDetailRepository;
            _stockRepository = stockRepository;
            _purchaseCartService = purchaseCartService;
            _supplierRepository = supplierRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseCartDetail>>> GetPurchaseCartDetails(int branchId, int companyId, int userId)
        {
            try
            {
                var details = await _purchaseCartDetailRepository.GetByDefaultSettingsAsync(branchId, companyId, userId);
                if (details == null) return NotFound();
                return Ok(details);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Domain.Models.Stock>> GetProductDetails(int id)
        {
            try
            {
                var product = await _stockRepository.GetByIdAsync(id);
                if (product == null) return NotFound();
                return Ok(new { product?.CurrentPurchaseUnitPrice });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddItem(PurchaseCartDetail item)
        {
            try
            {
                await _purchaseCartDetailRepository.AddAsync(item);
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
                await _purchaseCartDetailRepository.DeleteAsync(id);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<string>> CancelPurchase(int branchId, int companyId, int userId)
        {
            try
            {
                var details = await _purchaseCartDetailRepository.GetByDefaultSettingsAsync(branchId, companyId, userId);
                if (details == null) return BadRequest();
                await _purchaseCartDetailRepository.DeleteListAsync(details);
                return Ok(new { message = "Purchase canceled" });
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<int>> ConfirmPurchase(PurchaseConfirm purchaseConfirmDto)
        {
            try
            {
                var result = await _purchaseCartService.ConfirmPurchaseAsync(purchaseConfirmDto);

                return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}