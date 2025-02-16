using System;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/sale-cart")]
    public class SaleCartApiController : ApiController
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
        [Route("new-sale-details?branchId={branchId:int}&companyId={companyId:int}&userId={userId:int}")]
        public async Task<IHttpActionResult> GetNewSaleDetails([FromUri] int branchId, [FromUri] int companyId, [FromUri] int userId)
        {
            var details = await _saleCartDetailRepository.GetByDefaultSettingAsync(branchId, companyId, userId);
            return Ok(details);
        }

        [HttpGet]
        [Route("product-details/{id}")]
        public async Task<IHttpActionResult> GetProductDetails(int id)
        {
            var product = await _stockRepository.GetByIdAsync(id);
            if (product != null)
            {
                return Ok(new { data = product.SaleUnitPrice });
            }
            return Ok(new { data = 0 });
        }

        [HttpPost]
        [Route("add-item")]
        public async Task<IHttpActionResult> AddItem(SaleCartDetail newItem)
        {
            await _saleCartDetailRepository.AddAsync(newItem);
            return Ok("Item added successfully");
        }

        [HttpPost]
        [Route("delete-item/{id}")]
        public async Task<IHttpActionResult> DeleteItem(int id)
        {
            await _saleCartDetailRepository.DeleteAsync(id);
            return Ok("Item deleted successfully");
        }

        [HttpPost]
        [Route("cancel-sale?branchId={branchId:int}&companyId={companyId:int}&userId={userId:int}")]
        public async Task<IHttpActionResult> CancelSale([FromUri] int branchId, [FromUri] int companyId, [FromUri] int userId)
        {
            var saleDetails = await _saleCartDetailRepository.GetByDefaultSettingAsync(branchId, companyId, userId);
            await _saleCartDetailRepository.DeleteListAsync(saleDetails);
            return Ok("Sale canceled successfully");
        }

        [HttpPost]
        [Route("confirm-sale?branchId={branchId:int}&companyId={companyId:int}&userId={userId:int}")]
        public async Task<IHttpActionResult> ConfirmSale(SaleConfirm saleConfirmDto, [FromUri] int branchId, [FromUri] int companyId, [FromUri] int userId)
        {
            var result = await _saleCartService.ConfirmSaleAsync(saleConfirmDto, branchId, companyId, userId);
            if (result.IsSuccess)
            {
                return Ok(new { Success = true, Value = result.Value });
            }
            return Ok(new { Success = false, ErrorMessage = result.ErrorMessage });
        }
    }
}