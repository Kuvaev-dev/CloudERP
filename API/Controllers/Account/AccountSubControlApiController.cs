using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/account-sub-control")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountSubControlApiController : ApiController
    {
        private readonly IAccountSubControlRepository _accountSubControlRepository;
        private readonly IAccountControlRepository _accountControlRepository;
        private readonly IAccountHeadRepository _accountHeadRepository;

        public AccountSubControlApiController(
            IAccountSubControlRepository accountSubControlRepository,
            IAccountControlRepository accountControlRepository,
            IAccountHeadRepository accountHeadRepository)
        {
            _accountSubControlRepository = accountSubControlRepository ?? throw new ArgumentNullException(nameof(IAccountSubControlRepository));
            _accountControlRepository = accountControlRepository ?? throw new ArgumentNullException(nameof(IAccountControlRepository));
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(IAccountHeadRepository));
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAll([FromUri] int companyId, [FromUri] int branchId)
        {
            try
            {
                var subControls = await _accountSubControlRepository.GetAllAsync(companyId, branchId);
                return Ok(subControls);
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
                var subControl = await _accountSubControlRepository.GetByIdAsync(id);
                if (subControl == null) return NotFound();
                return Ok(subControl);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] AccountSubControl model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _accountSubControlRepository.AddAsync(model);
                return Created($"api/account-sub-control/{model.AccountSubControlID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}