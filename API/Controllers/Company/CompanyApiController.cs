using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Company
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class CompanyApiController : ControllerBase
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IFileService _fileService;
        private readonly IFileAdapterFactory _fileAdapterFactory;

        private const string DEFAULT_COMPANY_LOGO_PATH = "~/CompanyLogo/erp-logo.png";

        public CompanyApiController(
            ICompanyRepository companyRepository,
            IFileService fileService,
            IFileAdapterFactory fileAdapterFactory)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(ICompanyRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(IFileService));
            _fileAdapterFactory = fileAdapterFactory ?? throw new ArgumentNullException(nameof(IFileAdapterFactory));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Domain.Models.Company>>> GetAll()
        {
            try
            {
                var companies = await _companyRepository.GetAllAsync();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpGet]
        public async Task<ActionResult<Domain.Models.Company>> GetById(int id)
        {
            try
            {
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null) return NotFound("Model cannot be null.");
                return Ok(company);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Domain.Models.Company>> Create([FromBody] Domain.Models.Company model)
        {
            if (model == null)
                return BadRequest("Model cannot be null.");

            try
            {
                model.Logo ??= DEFAULT_COMPANY_LOGO_PATH;
                await _companyRepository.AddAsync(model);
                return CreatedAtAction(nameof(GetById), new { id = model.CompanyID }, model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating company: {ex.Message}");
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Domain.Models.Company model)
        {
            if (model == null)
                return BadRequest("Model cannot be null.");

            try
            {
                model.Logo ??= DEFAULT_COMPANY_LOGO_PATH;
                await _companyRepository.UpdateAsync(model);
                return Ok(model);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}