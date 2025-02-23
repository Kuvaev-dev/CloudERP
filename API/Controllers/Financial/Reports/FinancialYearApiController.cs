using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Financial.Reports
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class FinancialYearApiController : ControllerBase
    {
        private readonly IFinancialYearRepository _financialYearRepository;

        public FinancialYearApiController(IFinancialYearRepository financialYearRepository)
        {
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FinancialYear>>> GetAll()
        {
            try
            {
                var financialYears = await _financialYearRepository.GetAllAsync();
                if (financialYears == null) return NotFound();
                return Ok(financialYears);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<FinancialYear>> GetById(int id)
        {
            try
            {
                var financialYear = await _financialYearRepository.GetByIdAsync(id);
                if (financialYear == null) return NotFound();
                return Ok(financialYear);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<FinancialYear>> Create([FromBody] FinancialYear model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _financialYearRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.FinancialYearID }, model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] FinancialYear model)
        {
            if (model == null || id != model.FinancialYearID) return BadRequest("Invalid data.");

            try
            {
                await _financialYearRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}