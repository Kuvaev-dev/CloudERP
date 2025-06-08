using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Facades;

namespace API.Controllers.Account
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
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
                var settings = await _accountSettingFacade.AccountSettingRepository.GetAllAsync(companyId, branchId);
                return Ok(settings);
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
                var setting = await _accountSettingFacade.AccountSettingRepository.GetByIdAsync(id);
                if (setting == null) return NotFound("Model not found.");
                return Ok(setting);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<AccountSetting>> Create([FromBody] AccountSetting model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _accountSettingFacade.AccountSettingRepository.IsExists(model))
                    return Conflict("An account setting with the same details already exists.");

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
            if (model == null) return BadRequest("Model cannot be null.");
            if (id != model.AccountSettingID) return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _accountSettingFacade.AccountSettingRepository.IsExists(model))
                    return Conflict("An account setting with the same details already exists.");

                await _accountSettingFacade.AccountSettingRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}