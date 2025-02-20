using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;
using Utils.Interfaces;
using Utils.Models;

namespace API.Controllers.Financial.Forecasting
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ForecastingApiController : ControllerBase
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
        public ActionResult GenerateForecast([FromBody] ForecastInputModel inputModel)
        {
            if (inputModel == null) return BadRequest("Invalid forecast input data.");

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
                return Problem(detail: ex.Message, statusCode: 500);
            }
        }
    }
}