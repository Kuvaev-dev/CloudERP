using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Inventory
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class StockApiController : ControllerBase
    {
        private readonly IStockRepository _stockRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductQualityService _productQualityService;

        public StockApiController(
            IStockRepository stockRepository,
            ICategoryRepository categoryRepository,
            IProductQualityService productQualityService)
        {
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(IStockRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(ICategoryRepository));
            _productQualityService = productQualityService ?? throw new ArgumentNullException(nameof(IProductQualityService));
        }

        // GET: api/stock
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Stock>>> GetAll(int companyId, int branchId)
        {
            try
            {
                var stocks = await _stockRepository.GetAllAsync(companyId, branchId);
                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/stock/{id}
        [HttpGet]
        public async Task<ActionResult<Stock>> GetById(int id)
        {
            try
            {
                var stock = await _stockRepository.GetByIdAsync(id);
                if (stock == null) return NotFound("Model not found.");

                return Ok(stock);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // POST: api/stock
        [HttpPost]
        public async Task<ActionResult<Stock>> Create([FromBody] Domain.Models.Stock model)
        {
            try
            {
                if (await _stockRepository.IsExists(model))
                    return Conflict("A stock item with the same name already exists.");

                await _stockRepository.AddAsync(model);
                return CreatedAtRoute("GetById", new { id = model.ProductID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message + " " + ex.InnerException, statusCode: 500);
            }
        }

        // PUT: api/stock/{id}
        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] Domain.Models.Stock model)
        {
            if (model == null) 
                return BadRequest("Model cannot be null.");
            if (id != model.ProductID) 
                return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _stockRepository.IsExists(model))
                    return Conflict("A stock item with the same name already exists.");

                await _stockRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/stock/productquality
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductQuality>>> GetProductQuality(int companyId, int branchId)
        {
            try
            {
                var allProducts = await _productQualityService.GetAllProductsQualityAsync(branchId, companyId);
                return Ok(allProducts);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}