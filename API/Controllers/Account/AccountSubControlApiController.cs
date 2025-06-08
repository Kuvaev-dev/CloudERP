using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class AccountSubControlApiController : ControllerBase
    {
        private readonly IAccountSubControlRepository _accountSubControlRepository;
        private readonly IAccountControlRepository _accountControlRepository;
        private readonly IAccountHeadRepository _accountHeadRepository;

        public AccountSubControlApiController(
            IAccountSubControlRepository accountSubControlRepository,
            IAccountControlRepository accountControlRepository,
            IAccountHeadRepository accountHeadRepository)
        {
            _accountSubControlRepository = accountSubControlRepository ?? throw new ArgumentNullException(nameof(IAccountSubControlRepository));
            _accountControlRepository = accountControlRepository ?? throw new ArgumentNullException(nameof(IAccountControlRepository));
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(IAccountHeadRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountSubControl>>> GetAll(int companyId, int branchId)
        {
            try
            {
                var subControls = await _accountSubControlRepository.GetAllAsync(companyId, branchId);
                return Ok(subControls);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<AccountSubControl>> GetById(int id)
        {
            try
            {
                var subControl = await _accountSubControlRepository.GetByIdAsync(id);
                if (subControl == null) return NotFound("Model not found.");
                return Ok(subControl);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AccountSubControl>> Create([FromBody] AccountSubControl model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _accountSubControlRepository.IsExists(model))
                    return Conflict("An account sub-control with the same name already exists.");

                await _accountSubControlRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.AccountSubControlID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] AccountSubControl model)
        {
            if (model == null) return BadRequest("Model cannot be null.");
            if (id != model.AccountSubControlID) return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _accountSubControlRepository.IsExists(model))
                    return Conflict("An account sub-control with the same name already exists.");

                await _accountSubControlRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}