using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Stock
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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
        public async Task<ActionResult<IEnumerable<Domain.Models.Stock>>> GetAll(int companyId, int branchId)
        {
            try
            {
                var stocks = await _stockRepository.GetAllAsync(companyId, branchId);
                if (stocks == null) return NotFound();

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/stock/{id}
        [HttpGet]
        public async Task<ActionResult<Domain.Models.Stock>> GetById(int id)
        {
            try
            {
                var stock = await _stockRepository.GetByIdAsync(id);
                if (stock == null) return NotFound();

                return Ok(stock);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // POST: api/stock
        [HttpPost]
        public async Task<ActionResult<Domain.Models.Stock>> Create([FromBody] Domain.Models.Stock model, int companyId, int branchId, int userId)
        {
            try
            {
                model.CompanyID = companyId;
                model.BranchID = branchId;
                model.UserID = userId;

                if (ModelState.IsValid)
                {
                    var existingStock = await _stockRepository.GetByProductNameAsync(companyId, branchId, model.ProductName);
                    if (existingStock != null) return Conflict();

                    await _stockRepository.AddAsync(model);
                    return CreatedAtRoute("GetById", new { id = model.ProductID }, model);
                }

                return BadRequest("Invalid data.");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // PUT: api/stock/{id}
        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] Domain.Models.Stock model, int userId)
        {
            try
            {
                model.UserID = userId;

                if (ModelState.IsValid)
                {
                    var existingStock = await _stockRepository.GetByProductNameAsync(model.CompanyID, model.BranchID, model.ProductName);

                    if (existingStock != null && existingStock.ProductID != model.ProductID) return Conflict();

                    await _stockRepository.UpdateAsync(model);
                    return Ok(model);
                }

                return BadRequest("Invalid data.");
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
                if (allProducts == null) return NotFound();
                return Ok(allProducts);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}