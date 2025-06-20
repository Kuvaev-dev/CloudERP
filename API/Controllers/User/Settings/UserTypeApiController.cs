using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.User.Settings
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserTypeApiController : ControllerBase
    {
        private readonly IUserTypeRepository _userTypeRepository;

        public UserTypeApiController(IUserTypeRepository userTypeRepository)
        {
            _userTypeRepository = userTypeRepository ?? throw new ArgumentNullException(nameof(userTypeRepository));
        }

        // GET: api/user-types
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserType>>> GetAll()
        {
            try
            {
                var userTypes = await _userTypeRepository.GetAllAsync();
                return Ok(userTypes);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/user-types/5
        [HttpGet]
        public async Task<ActionResult<UserType>> GetById(int id)
        {
            try
            {
                var userType = await _userTypeRepository.GetByIdAsync(id);
                if (userType == null) return NotFound("Model not found.");
                return Ok(userType);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // POST: api/user-types
        [HttpPost]
        public async Task<ActionResult<UserType>> Create([FromBody] UserType model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _userTypeRepository.IsExists(model))
                    return Conflict("A user type with the same name already exists.");

                await _userTypeRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.UserTypeID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // PUT: api/user-types/5
        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] UserType model)
        {
            if (model == null) 
                return BadRequest("Model cannot be null.");
            if (id != model.UserTypeID) 
                return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _userTypeRepository.IsExists(model))
                    return Conflict("A user type with the same name already exists.");

                await _userTypeRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}