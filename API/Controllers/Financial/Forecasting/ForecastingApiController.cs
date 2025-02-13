using System;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.RepositoryAccess;
using Utils.Interfaces;
using Utils.Models;

namespace API.Controllers
{
    [RoutePrefix("api/forecasting")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ForecastingApiController : ApiController
    {
        private readonly IForecastingRepository _forecastingRepository;
        private readonly IForecastingService _forecastingService;

        public ForecastingApiController(
            IForecastingRepository forecastingRepository,
            IForecastingService forecastingService)
        {
            _forecastingRepository = forecastingRepository ?? throw new ArgumentNullException(nameof(IForecastingRepository));
            _forecastingService = forecastingService ?? throw new ArgumentNullException(nameof(IForecastingService));
        }

        [HttpPost]
        [Route("generate")]
        public IHttpActionResult GenerateForecast([FromBody] ForecastInputModel inputModel)
        {
            if (inputModel == null)
                return BadRequest("Invalid forecast input data.");

            try
            {
                inputModel.StartDate = DateTime.Now;
                var forecastValue = _forecastingService.GenerateForecast(
                    inputModel.CompanyID,
                    inputModel.BranchID,
                    inputModel.StartDate,
                    inputModel.EndDate);

                return Ok(new { Success = true, Forecast = forecastValue });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}