using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Branch
{
    [ApiController]
    public class BranchTypeApiController : ControllerBase
    {
        private readonly IBranchTypeRepository _branchTypeRepository;

        public BranchTypeApiController(IBranchTypeRepository branchTypeRepository)
        {
            _branchTypeRepository = branchTypeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchType>>> GetAll()
        {
            try
            {
                var branchTypes = await _branchTypeRepository.GetAllAsync();
                if (branchTypes == null) return NotFound();
                return Ok(branchTypes);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<BranchType>> GetById(int id)
        {
            try
            {
                var branchType = await _branchTypeRepository.GetByIdAsync(id);
                if (branchType == null) return NotFound();
                return Ok(branchType);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost, Route("create")]
        public async Task<ActionResult<BranchType>> Create([FromBody] BranchType model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _branchTypeRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.BranchTypeID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] BranchType model)
        {
            if (model == null || id != model.BranchTypeID) return BadRequest("Invalid data.");

            try
            {
                var existingBranchType = await _branchTypeRepository.GetByIdAsync(id);
                if (existingBranchType == null) return NotFound();

                await _branchTypeRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}