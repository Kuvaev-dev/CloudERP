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
                if (userTypes == null) return NotFound();
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
                if (userType == null) return NotFound();
                return Ok(userType);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // POST: api/user-types
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<UserType>> Create([FromBody] UserType model)
        {
            try
            {
                if (model == null) return BadRequest("Invalid data.");

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
            try
            {
                if (model == null || model.UserTypeID != id) return BadRequest("Invalid data.");

                var existingUserType = await _userTypeRepository.GetByIdAsync(id);
                if (existingUserType == null) return NotFound();

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