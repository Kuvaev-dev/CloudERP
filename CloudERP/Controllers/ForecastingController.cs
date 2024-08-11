using CloudERP.Helpers.Forecasting;
using System;
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

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            var model = _forecastingService.GetForecastData(companyID, branchID);

            return View(model);
        }

        [HttpPost]
        public ActionResult GenerateForecast()
        {
            if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
            {
                return RedirectToAction("Login", "Home");
            }

            int companyID = Convert.ToInt32(Session["CompanyID"]);
            int branchID = Convert.ToInt32(Session["BranchID"]);

            _forecastingService.GenerateForecast(companyID, branchID);
            var model = _forecastingService.GetForecastData(companyID, branchID);

            TempData["Message"] = "Forecast has been generated.";

            return View("Index", model);
        }
    }
}