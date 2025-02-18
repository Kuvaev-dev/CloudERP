using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;
using Utils.Helpers;

namespace API.Controllers
{
    [RoutePrefix("api/user-settings")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserSettingApiController : ApiController
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
        [Route("create-user")]
        public async Task<IHttpActionResult> CreateUser(User user, [FromUri] int companyId, [FromUri] int branchId)
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
                return InternalServerError(ex);
            }
        }

        // PUT: Update User
        [HttpPut]
        [Route("update-user")]
        public async Task<IHttpActionResult> UpdateUser(User user)
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
                return InternalServerError(ex);
            }
        }

        // GET: User details
        [HttpGet]
        [Route("user/{userId:int}")]
        public async Task<IHttpActionResult> GetUser(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}