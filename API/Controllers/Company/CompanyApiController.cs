using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.RepositoryAccess;
using Domain.Models;
using DatabaseAccess.Factories;
using Utils.Interfaces;
using System.Web;

namespace API.Controllers
{
    [RoutePrefix("api/company")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CompanyApiController : ApiController
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IFileService _fileService;
        private readonly IFileAdapterFactory _fileAdapterFactory;

        private const string COMPANY_LOGO_PATH = "~/Content/CompanyLogo";
        private const string DEFAULT_COMPANY_LOGO_PATH = "~/Content/CompanyLogo/erp-logo.png";

        public CompanyApiController(
            ICompanyRepository companyRepository,
            IFileService fileService,
            IFileAdapterFactory fileAdapterFactory)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(ICompanyRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(IFileService));
            _fileAdapterFactory = fileAdapterFactory ?? throw new ArgumentNullException(nameof(IFileAdapterFactory));
        }

        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var companies = await _companyRepository.GetAllAsync();
                return Ok(companies);
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
                var company = await _companyRepository.GetByIdAsync(id);
                if (company == null) return NotFound();
                return Ok(company);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] Company model, [FromBody] HttpPostedFile logo)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                if (await _companyRepository.CheckCompanyExistsAsync(model.Name))
                {
                    ModelState.AddModelError("Name", Localization.CloudERP.Messages.Messages.AlreadyExists);
                    return NotFound();
                }

                if (logo != null)
                {
                    var fileName = $"{model.Name}.jpg";

                    var fileAdapter = _fileAdapterFactory.Create(logo);
                    model.Logo = _fileService.UploadPhoto(fileAdapter, COMPANY_LOGO_PATH, fileName);
                }
                else
                {
                    model.Logo = _fileService.SetDefaultPhotoPath(DEFAULT_COMPANY_LOGO_PATH);
                }

                await _companyRepository.AddAsync(model);
                return Created($"api/company/{model.CompanyID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("update/{id:int}")]
        public async Task<IHttpActionResult> Update([FromBody] Company model, [FromBody] HttpPostedFile logo)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                if (await _companyRepository.CheckCompanyExistsAsync(model.Name))
                {
                    ModelState.AddModelError("Name", Localization.CloudERP.Messages.Messages.AlreadyExists);
                    return NotFound();
                }

                if (logo != null)
                {
                    var fileName = $"{model.Name}.jpg";

                    var fileAdapter = _fileAdapterFactory.Create(logo);
                    model.Logo = _fileService.UploadPhoto(fileAdapter, COMPANY_LOGO_PATH, fileName);
                }
                else
                {
                    model.Logo = _fileService.SetDefaultPhotoPath(DEFAULT_COMPANY_LOGO_PATH);
                }

                await _companyRepository.AddAsync(model);
                return Created($"api/company/{model.CompanyID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}