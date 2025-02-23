using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Stock
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
        public async Task<ActionResult<IEnumerable<Category>>> GetAll(int companyID, int branchID)
        {
            var categories = await _categoryRepository.GetAllAsync(companyID, branchID);
            if (categories == null) return NotFound();
            return Ok(categories);
        }

        [HttpGet]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category != null ? Ok(category) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create(Category model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _categoryRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.CategoryID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit(int id, [FromBody] Category model)
        {
            if (model == null || id != model.CategoryID) return BadRequest("Invalid data.");

            try
            {
                await _categoryRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}