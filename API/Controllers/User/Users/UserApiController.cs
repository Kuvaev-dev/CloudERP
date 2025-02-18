using System;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Models;
using Domain.RepositoryAccess;
using Utils.Helpers;

namespace API.Controllers
{
    [RoutePrefix("api/user")]
    public class UserApiController : ApiController
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
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                if (users == null) return NotFound();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/user/branch/{companyId}/{branchTypeId}/{branchId}
        [HttpGet]
        [Route("branch/{companyId:int}/{branchTypeId:int}/{branchId}")]
        public async Task<IHttpActionResult> GetByBranch(int companyId, int branchTypeId, int branchId)
        {
            try
            {
                var users = await _userRepository.GetByBranchAsync(companyId, branchTypeId, branchId);
                if (users == null) return NotFound();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/user/{id}
        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null) return NotFound();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/user/create
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] User model)
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
                return InternalServerError(ex);
            }
        }

        // PUT: api/user/update/{id}
        [HttpPut]
        [Route("update/{id}")]
        public async Task<IHttpActionResult> UpdateUser(int id, [FromBody] User model)
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

                await _userRepository.UpdateAsync(model);
                return Ok("User updated successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}