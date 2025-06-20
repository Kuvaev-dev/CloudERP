﻿using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Branch
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class BranchApiController : ControllerBase
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Models.Branch>>> GetAll(int companyId, int branchId, int mainBranchTypeID)
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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Models.Branch>>> GetSubBranches(int companyId, int branchId)
        {
            try
            {
                var subBranches = await _branchRepository.GetSubBranchAsync(companyId, branchId);
                return Ok(subBranches);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Models.Branch>>> GetByCompany(int companyId)
        {
            try
            {
                var branches = await _branchRepository.GetByCompanyAsync(companyId);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Domain.Models.Branch>> GetById(int id)
        {
            try
            {
                var branch = await _branchRepository.GetByIdAsync(id);
                return Ok(branch);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Models.Branch>> Create([FromBody] Domain.Models.Branch model)
        {
            if (model == null) return BadRequest("Model cannot be null.");

            try
            {
                if (await _branchRepository.IsExists(model))
                    return Conflict("A branch with the same name already exists.");

                await _branchRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetByCompany), new { companyId = model.CompanyID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] Domain.Models.Branch model)
        {
            if (model == null) 
                return BadRequest("Model cannot be null.");
            if (id != model.BranchID) 
                return BadRequest("ID in the request does not match the model ID.");

            try
            {
                if (await _branchRepository.IsExists(model))
                    return Conflict("A branch with the same name already exists.");

                await _branchRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}