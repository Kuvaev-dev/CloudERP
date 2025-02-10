using CloudERP.Helpers;
using Domain.RepositoryAccess;
using System;
using System.Web.Mvc;
using Utils.Interfaces;
using Utils.Models;

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
                EndDate = DateTime.Now
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
                var forecastValue = _forecastingService.GenerateForecast(
                    _sessionHelper.CompanyID, 
                    _sessionHelper.BranchID, 
                    inputModel.StartDate, 
                    inputModel.EndDate);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected Error is Occured^ " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}