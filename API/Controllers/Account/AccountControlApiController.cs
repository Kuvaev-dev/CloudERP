using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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
                if (accountControls == null) return NotFound();
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
                if (accountControl == null) return NotFound();
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
            if (model == null) return BadRequest();

            try
            {
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
            if (model == null || model.AccountControlID != id) return BadRequest();

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