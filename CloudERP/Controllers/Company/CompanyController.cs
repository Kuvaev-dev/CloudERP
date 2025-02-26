using CloudERP.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CloudERP.Controllers.Company
{
    public class CompanyController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public CompanyController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: Company
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var companies = await _httpClient.GetAsync<IEnumerable<Domain.Models.Company>>("companyapi/getall");
                return View(companies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Company/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null) return RedirectToAction("EP404", "EP");

                var company = await _httpClient.GetAsync<Domain.Models.Company>($"companyapi/getbyid?id={id.Value}");
                if (company == null) return RedirectToAction("EP404", "EP");

                return View(company);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new Domain.Models.Company());
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Domain.Models.Company model, IFormFile logo)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    using var content = new MultipartFormDataContent
                    {
                        { new StringContent(JsonConvert.SerializeObject(model)), "model" }
                    };

                    if (logo != null)
                    {
                        var stream = logo.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(logo.ContentType);
                        content.Add(fileContent, "file", logo.FileName);
                    }

                    var success = await _httpClient.PostAsync("companyapi/create", content);

                    if (success)
                    {
                        return RedirectToAction("Employee");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ошибка при регистрации сотрудника.";
                        return RedirectToAction("EP500", "EP");
                    }
                }

                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Company/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null) return RedirectToAction("EP404", "EP");

                var company = await _httpClient.GetAsync<Domain.Models.Company>($"companyapi/getbyid?id={id.Value}");
                if (company == null) return RedirectToAction("EP404", "EP");

                return View(company);
            }
            catch
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Domain.Models.Company model, IFormFile logo)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    using var content = new MultipartFormDataContent
                    {
                        { new StringContent(JsonConvert.SerializeObject(model)), "model" }
                    };

                    if (logo != null)
                    {
                        var stream = logo.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(logo.ContentType);
                        content.Add(fileContent, "file", logo.FileName);
                    }

                    var success = await _httpClient.PutAsync($"companyapi/update?id={model.CompanyID}", model);

                    if (success)
                    {
                        return RedirectToAction("Employee");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ошибка при регистрации сотрудника.";
                        return RedirectToAction("EP500", "EP");
                    }
                }

                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}