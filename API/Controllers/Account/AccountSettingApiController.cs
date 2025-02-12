using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Services.Facades;

namespace API.Controllers
{
    [RoutePrefix("api/account-setting")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountSettingApiController : ApiController
    {
        private readonly AccountSettingFacade _accountSettingFacade;

        public AccountSettingApiController(AccountSettingFacade accountSettingFacade)
        {
            _accountSettingFacade = accountSettingFacade;
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAll([FromUri] int companyId, [FromUri] int branchId)
        {
            try
            {
                var activities = await _accountSettingFacade.AccountSettingRepository.GetAllAsync(companyId, branchId);
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
                var activity = await _accountSettingFacade.AccountSettingRepository.GetByIdAsync(id);
                if (activity == null) return NotFound();
                return Ok(activity);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] AccountSetting model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _accountSettingFacade.AccountSettingRepository.AddAsync(model);
                return Created($"api/account-setting/{model.AccountSettingID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("update/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] AccountSetting model)
        {
            if (model == null || id != model.AccountSettingID) return BadRequest("Invalid data.");

            try
            {
                await _accountSettingFacade.AccountSettingRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}