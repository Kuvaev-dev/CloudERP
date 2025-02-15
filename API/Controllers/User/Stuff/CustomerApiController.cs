using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/customers")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerApiController : ApiController
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerApiController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        // GET: api/customers
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerRepository.GetAllAsync();
                if (customers == null)
                {
                    return NotFound();
                }
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/customers/5
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetCustomer(int id)
        {
            try
            {
                var customer = await _customerRepository.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/customers
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest("Invalid customer data.");
                }

                await _customerRepository.AddAsync(customer);
                return CreatedAtRoute("GetCustomer", new { id = customer.CustomerID }, customer);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/customers/5
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> UpdateCustomer(int id, [FromBody] Customer customer)
        {
            try
            {
                if (customer == null || customer.CustomerID != id)
                {
                    return BadRequest("Invalid customer data.");
                }

                var existingCustomer = await _customerRepository.GetByIdAsync(id);
                if (existingCustomer == null)
                {
                    return NotFound();
                }

                await _customerRepository.UpdateAsync(customer);
                return Ok(customer);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}