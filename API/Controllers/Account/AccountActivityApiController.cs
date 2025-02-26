using DatabaseAccess.Repositories.Branch;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class AccountActivityApiController : ControllerBase
    {
        private readonly IAccountActivityRepository _accountActivityRepository;

        public AccountActivityApiController(IAccountActivityRepository accountActivityRepository)
        {
            _accountActivityRepository = accountActivityRepository ?? throw new ArgumentNullException(nameof(accountActivityRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountActivity>>> GetAll()
        {
            try
            {
                var activities = await _accountActivityRepository.GetAllAsync();
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<AccountActivity>> GetById(int id)
        {
            try
            {
                var activity = await _accountActivityRepository.GetByIdAsync(id);
                if (activity == null) return NotFound("Model not found.");
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AccountActivity>> Create([FromBody] AccountActivity model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                await _accountActivityRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.AccountActivityID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] AccountActivity model)
        {
            if (model == null) return BadRequest("Model cannot be null.");
            if (id != model.AccountActivityID) return BadRequest("ID in the request does not match the model ID.");

            try
            {
                await _accountActivityRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}