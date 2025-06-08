using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class AccountControlApiController : ControllerBase
    {
        private readonly IAccountControlRepository _accountControlRepository;

        public AccountControlApiController(IAccountControlRepository accountControlRepository)
        {
            _accountControlRepository = accountControlRepository ?? throw new ArgumentNullException(nameof(accountControlRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountControl>>> GetAll(int companyId, int branchId)
        {
            try
            {
                var accountControls = await _accountControlRepository.GetAllAsync(companyId, branchId);
                return Ok(accountControls);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<AccountControl>> GetById(int id)
        {
            try
            {
                var accountControl = await _accountControlRepository.GetByIdAsync(id);
                if (accountControl == null) return NotFound("Model not found.");
                return Ok(accountControl);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AccountControl>> Create([FromBody] AccountControl model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _accountControlRepository.IsExists(model))
                    return Conflict("An account control with the same name already exists.");

                await _accountControlRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.AccountControlID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] AccountControl model)
        {
            if (model == null) return BadRequest("Model cannot be null.");
            if (id != model.AccountControlID) return BadRequest("ID in the request does not match the model ID.");
            if (await _accountControlRepository.IsExists(model))
                return Conflict("An account control with the same name already exists.");

            try
            {
                await _accountControlRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}