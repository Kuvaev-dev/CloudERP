using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Client
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CustomerApiController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerApiController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        // GET: api/customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetBySetting(int companyId, int branchId)
        {
            try
            {
                var customers = await _customerRepository.GetByCompanyAndBranchAsync(companyId, branchId);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/customers/5
        [HttpGet]
        public async Task<ActionResult<Customer>> GetById(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null) return NotFound("Model not found.");
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // POST: api/customers
        [HttpPost]
        public async Task<ActionResult<Customer>> Create([FromBody] Customer model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _customerRepository.IsExists(model))
                    return Conflict("A customer with the same name already exists.");

                await _customerRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.CustomerID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // PUT: api/customers/5
        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] Customer model)
        {
            if (model == null) 
                return BadRequest("Model cannot be null.");
            if (id != model.CustomerID) 
                return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _customerRepository.IsExists(model))
                    return Conflict("A customer with the same name already exists.");

                await _customerRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}