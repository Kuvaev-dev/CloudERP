using Domain.RepositoryAccess;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.User.Users
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserApiController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHelper _passwordHelper;

        public UserApiController(
            IUserRepository userRepository,
            IPasswordHelper passwordHelper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(passwordHelper));
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Models.User>>> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/user/branch/{companyId}/{branchTypeId}/{branchId}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Models.User>>> GetByBranch(int companyId, int branchTypeId, int branchId)
        {
            try
            {
                var users = await _userRepository.GetByBranchAsync(companyId, branchTypeId, branchId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: api/user/{id}
        [HttpGet]
        public async Task<ActionResult<Domain.Models.User>> GetById(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return NotFound("Model not found.");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // POST: api/user/create
        [HttpPost]
        public async Task<ActionResult<string>> Create([FromBody] Domain.Models.User model)
        {
            try
            {
                if (model == null) return BadRequest("Model cannot be null.");

                model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                model.Salt = salt;

                await _userRepository.AddAsync(model); 
                return CreatedAtAction(nameof(GetById), new { id = model.UserID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // PUT: api/user/update/{id}
        [HttpPut]
        public async Task<ActionResult<string>> Update(int id, [FromBody] Domain.Models.User model)
        {
            try
            {
                if (model == null) return BadRequest("Model cannot be null.");

                if (string.IsNullOrEmpty(model.Password))
                {
                    var existingUser = await _userRepository.GetByIdAsync(id);
                    model.Password = existingUser.Password;
                    model.Salt = existingUser.Salt;
                }
                else
                {
                    model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                    model.Salt = salt;
                }

                model.UserID = id;

                await _userRepository.UpdateAsync(model);

                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}