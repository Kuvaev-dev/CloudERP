using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Company
{
    public class CompanyController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;
        private readonly IImageUploadHelper _imageUploadHelper;

        private const string COMPANY_LOGO_FOLDER = "CompanyLogo";

        public CompanyController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IImageUploadHelper imageUploadHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _imageUploadHelper = imageUploadHelper ?? throw new ArgumentNullException(nameof(imageUploadHelper));
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
                    if (logo != null)
                        model.Logo = await _imageUploadHelper.UploadImageAsync(logo, COMPANY_LOGO_FOLDER);

                    var response = await _httpClient.PostAsync("companyapi/create", model);
                    if (response) return RedirectToAction("Index");
                    else ModelState.AddModelError("", "Failed to create company.");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                    if (logo != null)
                        model.Logo = await _imageUploadHelper.UploadImageAsync(logo, COMPANY_LOGO_FOLDER);

                    var response = await _httpClient.PutAsync("companyapi/update", model);
                    if (response) return RedirectToAction("Index");
                    else ModelState.AddModelError("", "Failed to create company.");
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