using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/user-type")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserTypeApiController : ApiController
    {
        private readonly IUserTypeRepository _userTypeRepository;

        public UserTypeApiController(IUserTypeRepository userTypeRepository)
        {
            _userTypeRepository = userTypeRepository ?? throw new ArgumentNullException(nameof(userTypeRepository));
        }

        // GET: api/user-types
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var userTypes = await _userTypeRepository.GetAllAsync();
                if (userTypes == null)
                {
                    return NotFound();
                }
                return Ok(userTypes);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // GET: api/user-types/5
        [HttpGet]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> GetById(int id)
        {
            try
            {
                var userType = await _userTypeRepository.GetByIdAsync(id);
                if (userType == null) return NotFound();

                return Ok(userType);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // POST: api/user-types
        [HttpPost]
        [Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] UserType userType)
        {
            try
            {
                if (userType == null) return BadRequest("Invalid data.");

                await _userTypeRepository.AddAsync(userType);
                return CreatedAtRoute("GetById", new { id = userType.UserTypeID }, userType);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        // PUT: api/user-types/5
        [HttpPut]
        [Route("update/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] UserType userType)
        {
            try
            {
                if (userType == null || userType.UserTypeID != id) return BadRequest("Invalid data.");

                var existingUserType = await _userTypeRepository.GetByIdAsync(id);
                if (existingUserType == null) return NotFound();

                await _userTypeRepository.UpdateAsync(userType);
                return Ok(userType);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}