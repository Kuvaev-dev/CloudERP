using CloudERP.Helpers;
using Domain.Interfaces;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class ForecastingController : Controller
    {
        private readonly IForecastingRepository _forecastingRepository;
        private readonly IForecastingService _forecastingService;
        private readonly SessionHelper _sessionHelper;

        public ForecastingController(
            IForecastingRepository forecastingRepository,
            IForecastingService forecastingService,
            SessionHelper sessionHelper)
        {
            _forecastingRepository = forecastingRepository ?? throw new ArgumentNullException(nameof(IForecastingRepository));
            _forecastingService = forecastingService ?? throw new ArgumentNullException(nameof(IForecastingService));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public ActionResult Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new ForecastInputModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(-1)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GenerateForecast(ForecastInputModel inputModel)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                inputModel.StartDate = DateTime.Now;
                var forecastValue = _forecastingService.GenerateForecast(_sessionHelper.CompanyID, _sessionHelper.BranchID, inputModel.StartDate, inputModel.EndDate);
                TempData["Message"] = $"{Resources.Messages.ForecastHasBeenGenerated} {forecastValue}";
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Index", "Forecasting");
            }
        }
    }
}