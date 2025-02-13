using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/financial-year")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class FinancialYearApiController : ApiController
    {
        private readonly IFinancialYearRepository _financialYearRepository;

        public FinancialYearApiController(IFinancialYearRepository financialYearRepository)
        {
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
        }

        [HttpGet, Route("all")]
        public async Task<IHttpActionResult> GetAll()
        {
            try
            {
                var accountHeads = await _financialYearRepository.GetAllAsync();
                return Ok(accountHeads);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route("create")]
        public async Task<IHttpActionResult> Create([FromBody] FinancialYear model)
        {
            if (model == null) return BadRequest("Invalid data.");

            try
            {
                await _financialYearRepository.AddAsync(model);
                return Created($"api/financial-year/{model.FinancialYearID}", model);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route("update/{id:int}")]
        public async Task<IHttpActionResult> Update(int id, [FromBody] FinancialYear model)
        {
            if (model == null || id != model.FinancialYearID) return BadRequest("Invalid data.");

            try
            {
                await _financialYearRepository.UpdateAsync(model);
                return Ok();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}