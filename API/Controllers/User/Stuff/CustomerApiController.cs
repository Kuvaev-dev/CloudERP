using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace API.Controllers.User.Stuff
{
    [ApiController]
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
                if (customers == null) return NotFound();
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
                if (customer == null) return NotFound();
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
            try
            {
                if (model == null) return BadRequest("Invalid customer data.");

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
        public async Task<IActionResult> Update(int id, [FromBody] Customer customer)
        {
            try
            {
                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null) return NotFound();

                await _customerRepository.UpdateAsync(customer);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}