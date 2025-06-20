using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Financial.Reports
{
    public class FinancialYearController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public FinancialYearController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var financialYears = await _httpClient.GetAsync<IEnumerable<FinancialYear>>("financialyearapi/getall");
                return View(financialYears);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new FinancialYear() { StartDate = DateTime.Now, EndDate = DateTime.Now } );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FinancialYear model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PostAsync("financialyearapi/create", model);
                    if (success) return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var financialYear = await _httpClient.GetAsync<FinancialYear>($"financialyearapi/getbyid?id={id}");
                if (financialYear == null) return RedirectToAction("EP404", "EP");

                return View(financialYear);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FinancialYear model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PutAsync($"financialyearapi/update?id={model.FinancialYearID}", model);
                    if (success) return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}