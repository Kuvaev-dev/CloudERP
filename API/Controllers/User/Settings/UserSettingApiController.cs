using Domain.RepositoryAccess;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.User.Settings
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class UserSettingApiController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IPasswordHelper _passwordHelper;

        public UserSettingApiController(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            IPasswordHelper passwordHelper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _userTypeRepository = userTypeRepository ?? throw new ArgumentNullException(nameof(IUserTypeRepository));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(IPasswordHelper));
        }

        // POST: Create User
        [HttpPost]
        public async Task<ActionResult<Domain.Models.User>> CreateUser(Domain.Models.User user, int companyId, int branchId)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest("Model is invalid.");

                var existingUser = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == user.Email && u.UserID != user.UserID);
                var existingEmployee = (await _employeeRepository.GetByBranchAsync(branchId, companyId))
                    .FirstOrDefault(u => u.Email == user.Email && u.UserID == user.UserID);

                if (existingUser != null || existingEmployee != null) return Conflict("User or employee already exists");

                await _userRepository.AddAsync(user);

                var employee = await _employeeRepository.GetByIdAsync(user.UserID);
                if (employee != null)
                {
                    employee.UserID = user.UserID;
                    await _employeeRepository.UpdateAsync(employee);
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // PUT: Update User
        [HttpPut]
        public async Task<ActionResult<Domain.Models.User>> UpdateUser(Domain.Models.User user)
        {
            if (user == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _userRepository.IsExists(user))
                    return Conflict("User with the same name already exists.");

                await _userRepository.UpdateAsync(user);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message + " " + ex.InnerException, statusCode: 500);
            }
        }

        // GET: User details
        [HttpGet]
        public async Task<ActionResult<Domain.Models.User>> GetUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return NotFound("Model cannot be null.");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}