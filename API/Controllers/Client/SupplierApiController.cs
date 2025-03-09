using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Client
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SupplierApiController : ControllerBase
    {
        private readonly ISupplierRepository _repository;

        public SupplierApiController(ISupplierRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAll()
        {
            try
            {
                var suppliers = await _repository.GetAllAsync();
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetBySetting(int companyId, int branchId)
        {
            try
            {
                var suppliers = await _repository.GetByCompanyAndBranchAsync(companyId, branchId);
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Supplier>> GetById(int id)
        {
            try
            {
                var supplier = await _repository.GetByIdAsync(id);
                if (supplier == null) return NotFound("Model not found.");
                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Supplier>> Create([FromBody] Supplier model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                await _repository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.SupplierID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] Supplier model)
        {
            if (model == null) return BadRequest("Model cannot be null.");
            if (id != model.SupplierID) return BadRequest("ID in the request does not match the model ID.");

            try
            {
                await _repository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetByBranch(int branchId)
        {
            try
            {
                var suppliers = await _repository.GetSuppliersByBranchesAsync(branchId);
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}