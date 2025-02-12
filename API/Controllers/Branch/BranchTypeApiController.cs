using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/branch-type")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BranchTypeApiController : ApiController
    {
        private readonly IBranchTypeRepository _branchTypeRepository;

        public BranchTypeApiController(IBranchTypeRepository branchTypeRepository)
        {
            _branchTypeRepository = branchTypeRepository;
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var branchTypes = await _branchTypeRepository.GetAllAsync();
                return Ok(branchTypes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            try
            {
                var branchType = await _branchTypeRepository.GetByIdAsync(id);
                if (branchType == null) return NotFound();
                return Ok(branchType);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] BranchType model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _branchTypeRepository.AddAsync(model);
                return Created($"api/branch-type/{model.BranchTypeID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("update/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] BranchType model)
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
                return InternalServerError(ex);
            }
        }
    }
}