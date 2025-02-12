using System;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/account-control")]
    public class AccountControlApiController : ApiController
    {
        private readonly IAccountControlRepository _accountControlRepository;

        public AccountControlApiController(IAccountControlRepository accountControlRepository)
        {
            _accountControlRepository = accountControlRepository ?? throw new ArgumentNullException(nameof(accountControlRepository));
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAll([FromUri] int companyId, [FromUri] int branchId)
        {
            try
            {
                var accountControls = await _accountControlRepository.GetAllAsync(companyId, branchId);
                return Ok(accountControls);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("{id:int}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            try
            {
                var accountControl = await _accountControlRepository.GetByIdAsync(id);
                if (accountControl == null) return NotFound();

                return Ok(accountControl);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] AccountControl model)
        {
            if (model == null) return BadRequest("Invalid model");

            try
            {
                await _accountControlRepository.AddAsync(model);
                return Created($"api/account-control/{model.AccountControlID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("update/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] AccountControl model)
        {
            if (model == null || model.AccountControlID != id) return BadRequest("Invalid model");

            try
            {
                await _accountControlRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}