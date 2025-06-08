using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Inventory
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CategoryApiController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryApiController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll(int companyId, int branchId)
        {
            var categories = await _categoryRepository.GetAllAsync(companyId, branchId);
            return Ok(categories);
        }

        [HttpGet]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? Ok(category) : NotFound("Model not found.");
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create(Category model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _categoryRepository.IsExists(model))
                    return Conflict("A category with the same name already exists.");

                await _categoryRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.CategoryID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] Category model)
        {
            if (model == null) return BadRequest("Model cannot be null.");
            if (id != model.CategoryID) return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _categoryRepository.IsExists(model))
                    return Conflict("A category with the same name already exists.");

                await _categoryRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}