using CloudERP.Helpers.Forecasting;
using CloudERP.Models;
using CloudERP.Models.Forecasting;
using System;
using System.Linq;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class ForecastingController : Controller
    {
        private readonly ForecastingService _forecastingService;

        public ForecastingController(ForecastingService forecastingService)
        {
            _forecastingService = forecastingService;
        }

        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            var model = new ForecastInputModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(-1)
            };

            return View(model);
        }

        [HttpPost]
        public ActionResult GenerateForecast(ForecastInputModel inputModel)
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            try
            {
                inputModel.StartDate = DateTime.Now;

                var forecastData = _forecastingService.GetForecastData(companyID, branchID, inputModel.StartDate, inputModel.EndDate);
                var forecastValue = _forecastingService.GenerateForecast(companyID, branchID, inputModel.StartDate, inputModel.EndDate);

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