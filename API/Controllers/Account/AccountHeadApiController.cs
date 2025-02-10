using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/account-head")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountHeadApiController : ApiController
    {
        private readonly IAccountHeadRepository _repository;

        public AccountHeadApiController(IAccountHeadRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var accountHeads = await _repository.GetAllAsync();
                return Ok(accountHeads);
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
                var accountHead = await _repository.GetByIdAsync(id);
                if (accountHead == null) return NotFound();
                return Ok(accountHead);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Create([FromBody] AccountHead model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _repository.AddAsync(model);
                return Created($"api/account-head/{model.AccountHeadID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] AccountHead model)
        {
            if (model == null || id != model.AccountHeadID) return BadRequest("Invalid data.");

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
    }
}