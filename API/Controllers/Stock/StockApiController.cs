using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace API.Controllers
{
    [RoutePrefix("api/stock")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StockApiController : ApiController
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
        [HttpGet, Route("{companyId:int}/{branchId:int}")]
        public async Task<IHttpActionResult> GetAllStocks(int companyId, int branchId)
        {
            try
            {
                var stocks = await _stockRepository.GetAllAsync(companyId, branchId);
                if (stocks == null) return NotFound();

                return Ok(stocks);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Unexpected error occurred: " + ex.Message));
            }
        }

        // GET: api/stock/{id}
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetStockById(int id)
        {
            try
            {
                var stock = await _stockRepository.GetByIdAsync(id);
                if (stock == null) return NotFound();

                return Ok(stock);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Unexpected error occurred: " + ex.Message));
            }
        }

        // POST: api/stock
        [HttpPost, Route("create/{companyId:int}/{branchId:int}/{userId:int}")]
        public async Task<IHttpActionResult> CreateStock([FromBody] Stock model, int companyId, int branchId, int userId)
        {
            try
            {
                model.CompanyID = companyId;
                model.BranchID = branchId;
                model.UserID = userId;

                if (ModelState.IsValid)
                {
                    var existingStock = await _stockRepository.GetByProductNameAsync(companyId, branchId, model.ProductName);
                    if (existingStock != null)
                    {
                        return Conflict();
                    }

                    await _stockRepository.AddAsync(model);
                    return CreatedAtRoute("GetStockById", new { id = model.ProductID }, model);
                }

                return BadRequest("Invalid data.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Unexpected error occurred: " + ex.Message));
            }
        }

        // PUT: api/stock/{id}
        [HttpPut]
        [Route("{id:int}/{userId:int}")]
        public async Task<IHttpActionResult> UpdateStock(int id, [FromBody] Stock model, int userId)
        {
            try
            {
                model.UserID = userId;

                if (ModelState.IsValid)
                {
                    var existingStock = await _stockRepository.GetByProductNameAsync(model.CompanyID, model.BranchID, model.ProductName);

                    if (existingStock != null && existingStock.ProductID != model.ProductID)
                    {
                        return Conflict();
                    }

                    await _stockRepository.UpdateAsync(model);
                    return Ok(model);
                }

                return BadRequest("Invalid data.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Unexpected error occurred: " + ex.Message));
            }
        }

        // GET: api/stock/productquality
        [HttpGet]
        [Route("product-quality/{companyId:int]/{branchId:int}")]
        public async Task<IHttpActionResult> GetProductQuality(int companyId, int branchId)
        {
            try
            {
                var allProducts = await _productQualityService
                    .GetAllProductsQualityAsync(branchId, companyId);

                return Ok(allProducts);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Unexpected error occurred: " + ex.Message));
            }
        }
    }
}