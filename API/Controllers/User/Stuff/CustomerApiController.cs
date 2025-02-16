using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/customer")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CustomerApiController : ApiController
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerApiController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        // GET: api/customers
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
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
        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
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
        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    return BadRequest("Invalid customer data.");
                }

                await _customerRepository.AddAsync(customer);
                return CreatedAtRoute("GetById", new { id = customer.CustomerID }, customer);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/customers/5
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] Customer customer)
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
                return InternalServerError(ex);
            }
        }
    }
}