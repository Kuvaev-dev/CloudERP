using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Branch
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
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
                if (branchType == null) return NotFound("Model cannot be null.");
                return Ok(branchType);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<BranchType>> Create([FromBody] BranchType model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _branchTypeRepository.IsExists(model))
                    return Conflict("A branch type with the same name already exists.");

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
            if (model == null) return BadRequest("Model cannot be null.");
            if (id != model.BranchTypeID) return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _branchTypeRepository.IsExists(model))
                    return Conflict("A branch type with the same name already exists.");

                await _branchTypeRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}