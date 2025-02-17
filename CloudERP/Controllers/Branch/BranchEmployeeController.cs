using CloudERP.Helpers;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BranchEmployeeController : Controller
    {
        private readonly SessionHelper _sessionHelper;
        private readonly HttpClientHelper _httpClient;

        public BranchEmployeeController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Employee
        public async Task<ActionResult> Employee()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branches = await _httpClient.GetAsync<List<Employee>>(
                    $"branch-employee/employee/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                if (branches == null) return RedirectToAction("EP404", "EP");

                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: EmployeeRegistration
        public ActionResult EmployeeRegistration() => View(new Employee());

        // POST: EmployeeRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeRegistration(Employee model, HttpPostedFileBase logo)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.RegistrationDate = DateTime.Now;
                model.IsFirstLogin = true;

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

                        await _httpClient.PostAsync("branch-employee/registration", content);
                    }

                    return RedirectToAction("Employee");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: EmployeeUpdation
        public async Task<ActionResult> EmployeeUpdation(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var employee = await _httpClient.GetAsync<Employee>($"branch-employee/{id}");
                if (employee == null) return RedirectToAction("EP404", "EP");

                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: EmployeeUpdation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeUpdation(Employee model, HttpPostedFileBase logo)
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

                        await _httpClient.PostAsync("branch-employee/updation", content);
                    }

                    return RedirectToAction("Employee");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: ViewProfile
        public async Task<ActionResult> ViewProfile(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var employee = await _httpClient.GetAsync<Employee>($"branch-employee/{id}");
                if (employee == null) return RedirectToAction("EP404", "EP");

                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}