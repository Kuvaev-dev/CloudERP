using System;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/category")]
    public class CategoryApiController : ApiController
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryApiController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        [HttpGet]
        [Route("all/{companyID}/{branchID}")]
        public async Task<IHttpActionResult> GetAll(int companyID, int branchID)
        {
            var categories = await _categoryRepository.GetAllAsync(companyID, branchID);
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? Ok(category) : (IHttpActionResult)NotFound();
        }

        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create(Category category)
        {
            await _categoryRepository.AddAsync(category);
            return Ok(new { Message = "Category created successfully" });
        }

        [HttpPut]
        [Route("edit")]
        public async Task<IHttpActionResult> Edit(Category category)
        {
            await _categoryRepository.UpdateAsync(category);
            return Ok(new { Message = "Category updated successfully" });
        }
    }
}