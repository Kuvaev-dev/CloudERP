using CloudERP.Helpers;
using Domain.Models.Forecasting;
using Domain.RepositoryAccess;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class ForecastingController : Controller
    {
        private readonly IForecastingRepository _forecastingRepository;
        private readonly ForecastingService _forecastingService;
        private readonly SessionHelper _sessionHelper;

        public ForecastingController(IForecastingRepository forecastingRepository, ForecastingService forecastingService, SessionHelper sessionHelper)
        {
            _forecastingRepository = forecastingRepository;
            _forecastingService = forecastingService;
            _sessionHelper = sessionHelper;
        }

        public ActionResult Index()
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            return View(new ForecastInputModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(-1)
            });
        }

        [HttpPost]
        public ActionResult GenerateForecast(ForecastInputModel inputModel)
        {
            if (!_sessionHelper.IsAuthenticated)
            {
                return RedirectToAction("Login", "Home");
            }

            try
            {
                inputModel.StartDate = DateTime.Now;

                var forecastData = _forecastingRepository.GetForecastData(_sessionHelper.CompanyID, _sessionHelper.BranchID, inputModel.StartDate, inputModel.EndDate);
                var forecastValue = _forecastingService.GenerateForecast(_sessionHelper.CompanyID, _sessionHelper.BranchID, inputModel.StartDate, inputModel.EndDate);

                TempData["Message"] = $"{Resources.Messages.ForecastHasBeenGenerated} {forecastValue}";
                inputModel.ForecastData = forecastData;
                return View("Index", inputModel);
            }
            catch (InvalidOperationException ex)
            {
                TempData["Message"] = ex.Message;
                inputModel.ForecastData = Enumerable.Empty<ForecastData>();
                return View("Index", inputModel);
            }
        }
    }
}