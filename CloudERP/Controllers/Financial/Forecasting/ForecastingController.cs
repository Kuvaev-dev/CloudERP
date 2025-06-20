﻿using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Financial.Forecasting
{
    public class ForecastingController : Controller
    {
        private readonly ISessionHelper _sessionHelper;
        private readonly IHttpClientHelper _httpClient;

        public ForecastingController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
        public async Task<ActionResult> GenerateForecast(ForecastInputModel inputModel)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                inputModel.StartDate = DateTime.Now;
                inputModel.CompanyID = _sessionHelper.CompanyID;
                inputModel.BranchID = _sessionHelper.BranchID;

                var isSuccess = await _httpClient.PostAsync("forecastingapi/generateforecast", inputModel);

                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Forecast generated successfully!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "Failed to generate forecast.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Unexpected error occurred: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}