using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utils.Helpers;

namespace API.Controllers.User.Users
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserApiController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly PasswordHelper _passwordHelper;

        public UserApiController(
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            PasswordHelper passwordHelper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _userTypeRepository = userTypeRepository ?? throw new ArgumentNullException(nameof(userTypeRepository));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(passwordHelper));
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Models.User>>> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                if (users == null) return NotFound();
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
                if (users == null) return NotFound();

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
                if (user == null) return NotFound();
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
                if (model == null) return BadRequest("Invalid data.");

                model.Password = _passwordHelper.HashPassword(model.Password, out string salt);
                model.Salt = salt;

                await _userRepository.AddAsync(model);
                return Ok("User created successfully.");
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
                if (model == null) return BadRequest("Invalid data.");

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
                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}