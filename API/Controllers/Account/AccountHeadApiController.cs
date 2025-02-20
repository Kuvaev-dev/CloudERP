using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Account
{
    [ApiController]
    public class AccountHeadApiController : ControllerBase
    {
        private readonly IAccountHeadRepository _accountHeadRepository;

        public AccountHeadApiController(IAccountHeadRepository accountHeadRepository)
        {
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(accountHeadRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountHead>>> GetAll()
        {
            try
            {
                var accountHeads = await _accountHeadRepository.GetAllAsync();
                if (accountHeads == null) return NotFound();
                return Ok(accountHeads);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<AccountHead>> GetById(int id)
        {
            try
            {
                var accountHead = await _accountHeadRepository.GetByIdAsync(id);
                if (accountHead == null) return NotFound();
                return Ok(accountHead);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AccountHead>> Create([FromBody] AccountHead model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _accountHeadRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.AccountHeadID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] AccountHead model)
        {
            if (model == null || id != model.AccountHeadID) return BadRequest("Invalid data.");

            try
            {
                await _accountHeadRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}