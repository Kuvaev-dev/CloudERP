using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CloudERP.Controllers.Branch
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
        public async Task<ActionResult> EmployeeRegistration(Employee model, IFormFile logo)
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

                    var success = await _httpClient.PostAsync<Employee>("branch-employee/registration", content);

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
        public async Task<ActionResult> EmployeeUpdation(Employee model, IFormFile logo)
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

                    var success = await _httpClient.PutAsync<Employee>("branch-employee/updation", content);

                    if (success)
                    {
                        return RedirectToAction("Employee");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Ошибка при обновлении сотрудника.";
                        return RedirectToAction("EP500", "EP");
                    }
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