using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/account-activity")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountActivityApiController : ApiController
    {
        private readonly IAccountActivityRepository _repository;

        public AccountActivityApiController(IAccountActivityRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var activities = await _repository.GetAllAsync();
                return Ok(activities);
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
                var activity = await _repository.GetByIdAsync(id);
                if (activity == null) return NotFound();
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] AccountActivity model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _repository.AddAsync(model);
                return Created(Request.RequestUri + "/" + model.AccountActivityID, model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("update/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] AccountActivity model)
        {
            if (model == null || id != model.AccountActivityID) return BadRequest("Invalid data.");

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