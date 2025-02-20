using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Services.Facades;

namespace API.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountSettingApiController : ControllerBase
    {
        private readonly AccountSettingFacade _accountSettingFacade;

        public AccountSettingApiController(AccountSettingFacade accountSettingFacade)
        {
            _accountSettingFacade = accountSettingFacade ?? throw new ArgumentException(nameof(accountSettingFacade));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccountSetting>>> GetAll(int companyId, int branchId)
        {
            try
            {
                var activities = await _accountSettingFacade.AccountSettingRepository.GetAllAsync(companyId, branchId);
                return Ok(activities);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<AccountSetting>> GetById(int id)
        {
            try
            {
                var activity = await _accountSettingFacade.AccountSettingRepository.GetByIdAsync(id);
                if (activity == null) return NotFound();
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AccountSetting>> Create([FromBody] AccountSetting model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _accountSettingFacade.AccountSettingRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.AccountSettingID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] AccountSetting model)
        {
            if (model == null || id != model.AccountSettingID) return BadRequest("Invalid data.");

            try
            {
                await _accountSettingFacade.AccountSettingRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}