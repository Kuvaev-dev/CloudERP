using DatabaseAccess.Repositories.Customers;
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
        private readonly ISupplierRepository _supplierRepository;

        public SupplierApiController(ISupplierRepository repository)
        {
            _supplierRepository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetAll()
        {
            try
            {
                var suppliers = await _supplierRepository.GetAllAsync();
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Supplier>>> GetFromSubBranch(int branchId)
        {
            try
            {
                var suppliers = await _supplierRepository.GetSuppliersByBranchesAsync(branchId);
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
                var suppliers = await _supplierRepository.GetByCompanyAndBranchAsync(companyId, branchId);
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
                var supplier = await _supplierRepository.GetByIdAsync(id);
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
                if (await _supplierRepository.IsExists(model))
                    return Conflict("A supplier with the same name and contact number already exists.");

                await _supplierRepository.AddAsync(model);
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
            if (model == null) 
                return BadRequest("Model cannot be null.");
            if (id != model.SupplierID) 
                return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _supplierRepository.IsExists(model))
                    return Conflict("A supplier with the same name and contact number already exists.");

                await _supplierRepository.UpdateAsync(model);
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
                var suppliers = await _supplierRepository.GetSuppliersByBranchesAsync(branchId);
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}