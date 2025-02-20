using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.User.Stuff
{
    [ApiController]
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
                if (suppliers == null) return NotFound();
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
                if (supplier == null) return NotFound();
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
            if (model == null) return BadRequest("Invalid data.");

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
            if (model == null || id != model.SupplierID) return BadRequest("Invalid data.");

            try
            {
                await _repository.UpdateAsync(model);
                return Ok();
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
                if (suppliers == null) return NotFound();
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}