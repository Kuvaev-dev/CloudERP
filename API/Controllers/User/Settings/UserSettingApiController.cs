using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Utils.Helpers;

namespace API.Controllers.User.Settings
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserSettingApiController : ControllerBase
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly PasswordHelper _passwordHelper;

        public UserSettingApiController(
            IEmployeeRepository employeeRepository,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository,
            PasswordHelper passwordHelper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _userTypeRepository = userTypeRepository ?? throw new ArgumentNullException(nameof(IUserTypeRepository));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(PasswordHelper));
        }

        // POST: Create User
        [HttpPost]
        public async Task<ActionResult<Domain.Models.User>> CreateUser(Domain.Models.User user, int companyId, int branchId)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Model is invalid.");

                var existingUser = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == user.Email && u.UserID != user.UserID);
                var existingEmployee = (await _employeeRepository.GetByBranchAsync(branchId, companyId))
                    .FirstOrDefault(u => u.Email == user.Email && u.UserID == user.UserID);

                if (existingUser != null || existingEmployee != null) return Conflict();

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
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Model is invalid.");

                var existingUser = (await _userRepository.GetAllAsync())
                    .FirstOrDefault(u => u.Email == user.Email && u.UserID != user.UserID);

                if (existingUser != null) return Conflict();

                await _userRepository.UpdateAsync(user);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        // GET: User details
        [HttpGet]
        public async Task<ActionResult<Domain.Models.User>> GetUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}