using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.RepositoryAccess;
using Domain.Models;

namespace API.Controllers
{
    [RoutePrefix("api/branch")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BranchApiController : ApiController
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchTypeRepository _branchTypeRepository;
        private readonly IAccountSettingRepository _accountSettingRepository;

        private const int MAIN_BRANCH_TYPE_ID = 1;

        public BranchApiController(
            IBranchRepository branchRepository,
            IBranchTypeRepository branchTypeRepository,
            IAccountSettingRepository accountSettingRepository)
        {
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(IBranchRepository));
            _branchTypeRepository = branchTypeRepository ?? throw new ArgumentNullException(nameof(IBranchTypeRepository));
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(IAccountSettingRepository));
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAll([FromUri] int companyId, [FromUri] int branchId, [FromUri] int mainBranchTypeID)
        {
            try
            {
                var branches = mainBranchTypeID == MAIN_BRANCH_TYPE_ID
                    ? await _branchRepository.GetByCompanyAsync(companyId)
                    : await _branchRepository.GetSubBranchAsync(companyId, branchId);

                return Ok(branches);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("sub-branches")]
        public async Task<IHttpActionResult> GetSubBranches([FromUri] int companyId, [FromUri] int branchId)
        {
            try
            {
                var subBranches = await _branchRepository.GetSubBranchAsync(companyId, branchId);
                return Ok(subBranches);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route("company/{companyId:int}")]
        public async Task<IHttpActionResult> GetByCompanyAsync([FromUri] int companyId)
        {
            try
            {
                var branches = await _branchRepository.GetByCompanyAsync(companyId);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] Branch model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _branchRepository.AddAsync(model);
                return Created($"api/branch/{model.BranchID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] Branch model)
        {
            if (model == null || id != model.BranchID) return BadRequest("Invalid data.");

            try
            {
                var existingBranch = await _branchRepository.GetByIdAsync(id);
                if (existingBranch == null) return NotFound();

                await _branchRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}