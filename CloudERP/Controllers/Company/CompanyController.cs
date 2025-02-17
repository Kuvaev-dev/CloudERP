using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;

namespace CloudERP.Controllers
{
    public class CompanyController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public CompanyController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Company
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var companies = await _httpClient.GetAsync<List<Company>>("company");
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

                var company = await _httpClient.GetAsync<Company>($"company/{id.Value}");
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

            return View(new Company());
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Company model, HttpPostedFileBase logo)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(model)), "model");

                        if (logo != null)
                        {
                            var stream = logo.InputStream;
                            var fileContent = new StreamContent(stream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(logo.ContentType);
                            content.Add(fileContent, "file", logo.FileName);
                        }

                        await _httpClient.PostAsync("company/create", content);
                    }

                    return RedirectToAction("Index");
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

                var company = await _httpClient.GetAsync<Company>($"company/{id.Value}");
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
        public async Task<ActionResult> Edit(Company model, HttpPostedFileBase logo)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(model)), "model");

                        if (logo != null)
                        {
                            var stream = logo.InputStream;
                            var fileContent = new StreamContent(stream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(logo.ContentType);
                            content.Add(fileContent, "file", logo.FileName);
                        }

                        await _httpClient.PutAsync($"company/update/{model.CompanyID}", model);
                    }

                    return RedirectToAction("Index");
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