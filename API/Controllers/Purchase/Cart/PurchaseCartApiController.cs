using System;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models;
using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/purchase-cart")]
    public class PurchaseCartApiController : ApiController
    {
        private readonly IPurchaseCartDetailRepository _purchaseCartDetailRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IPurchaseCartService _purchaseCartService;

        public PurchaseCartApiController(
            IPurchaseCartDetailRepository purchaseCartDetailRepository,
            IStockRepository stockRepository,
            IPurchaseCartService purchaseCartService)
        {
            _purchaseCartDetailRepository = purchaseCartDetailRepository;
            _stockRepository = stockRepository;
            _purchaseCartService = purchaseCartService;
        }

        [HttpGet]
        [Route("details/{branchId:int}/{companyId:int}/{userId:int}")]
        public async Task<IHttpActionResult> GetPurchaseCartDetails(int branchId, int companyId, int userId)
        {
            try
            {
                var details = await _purchaseCartDetailRepository.GetByDefaultSettingsAsync(branchId, companyId, userId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("product/{id}")]
        public async Task<IHttpActionResult> GetProductDetails([FromUri] int id)
        {
            try
            {
                var product = await _stockRepository.GetByIdAsync(id);
                return Ok(new { product?.CurrentPurchaseUnitPrice });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("additem")]
        public async Task<IHttpActionResult> AddItem(PurchaseCartDetail item)
        {
            try
            {
                await _purchaseCartDetailRepository.AddAsync(item);
                return Ok(new { message = "Item added successfully" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IHttpActionResult> DeleteItem([FromUri] int id)
        {
            try
            {
                var item = await _purchaseCartDetailRepository.GetByIdAsync(id);
                if (item != null)
                {
                    await _purchaseCartDetailRepository.DeleteAsync(item);
                    return Ok(new { message = "Deleted successfully" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("cancel/{branchId:int}/{companyId:int}/{userId:int}")]
        public async Task<IHttpActionResult> CancelPurchase(int branchId, int companyId, int userId)
        {
            try
            {
                var details = await _purchaseCartDetailRepository.GetByDefaultSettingsAsync(branchId, companyId, userId);
                await _purchaseCartDetailRepository.DeleteListAsync(details);
                return Ok(new { message = "Purchase canceled" });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("confirm")]
        public async Task<IHttpActionResult> ConfirmPurchase(PurchaseConfirm purchaseConfirmDto)
        {
            try
            {
                var result = await _purchaseCartService.ConfirmPurchaseAsync(
                    purchaseConfirmDto, 
                    purchaseConfirmDto.CompanyID, 
                    purchaseConfirmDto.BranchID, 
                    purchaseConfirmDto.UserID);

                if (result.IsSuccess) return Ok(new { id = result.Value });

                return BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}