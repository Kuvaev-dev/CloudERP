using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Sale.Cart
{
    [ApiController]
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
        public async Task<ActionResult<IEnumerable<SaleCartDetail>>> GetNewSaleDetails(int branchId, int companyId, int userId)
        {
            var details = await _saleCartDetailRepository.GetByDefaultSettingAsync(branchId, companyId, userId);
            if (details == null) return NotFound();
            return Ok(details);
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetProductDetails(int id)
        {
            var product = await _stockRepository.GetByIdAsync(id);
            if (product != null)
            {
                return Ok(new { data = product.SaleUnitPrice });
            }
            return Ok(new { data = 0 });
        }

        [HttpPost]
        public async Task<ActionResult<string>> AddItem(SaleCartDetail newItem)
        {
            await _saleCartDetailRepository.AddAsync(newItem);
            return Ok("Item added successfully");
        }

        [HttpPost]
        public async Task<ActionResult<string>> DeleteItem(int id)
        {
            await _saleCartDetailRepository.DeleteAsync(id);
            return Ok("Item deleted successfully");
        }

        [HttpPost]
        public async Task<ActionResult<string>> CancelSale(int branchId, int companyId, int userId)
        {
            var saleDetails = await _saleCartDetailRepository.GetByDefaultSettingAsync(branchId, companyId, userId);
            await _saleCartDetailRepository.DeleteListAsync(saleDetails);
            return Ok("Sale canceled successfully");
        }

        [HttpPost]
        public async Task<ActionResult<object>> ConfirmSale(SaleConfirm saleConfirmDto, int branchId, int companyId, int userId)
        {
            var result = await _saleCartService.ConfirmSaleAsync(saleConfirmDto, branchId, companyId, userId);
            if (result.IsSuccess)
            {
                return Ok(new { Success = true, result.Value });
            }
            return Ok(new { Success = false, result.ErrorMessage });
        }
    }
}