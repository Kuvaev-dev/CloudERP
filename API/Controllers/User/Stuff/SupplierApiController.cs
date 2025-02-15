using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/supplier")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SupplierApiController : ApiController
    {
        private readonly ISupplierRepository _repository;

        public SupplierApiController(ISupplierRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAllSuppliers()
        {
            try
            {
                var suppliers = await _repository.GetAllAsync();
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetSupplierById(int id)
        {
            try
            {
                var supplier = await _repository.GetByIdAsync(id);
                if (supplier == null) return NotFound();
                return Ok(supplier);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> CreateSupplier([FromBody] Supplier model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _repository.AddAsync(model);
                return Created($"api/supplier/{model.SupplierID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("update/{id:int}")]
        public async Task<IHttpActionResult> UpdateSupplier(int id, [FromBody] Supplier model)
        {
            if (model == null || id != model.SupplierID) return BadRequest("Invalid data.");

            try
            {
                await _repository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("branch/{branchId:int}")]
        public async Task<IHttpActionResult> GetSuppliersByBranch(int branchId)
        {
            try
            {
                var suppliers = await _repository.GetSuppliersByBranchesAsync(branchId);
                return Ok(suppliers);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}