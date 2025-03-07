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
                var branches = await _httpClient.GetAsync<IEnumerable<Employee>>(
                    $"branchemployeeapi/employee?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: EmployeeRegistration
        public ActionResult EmployeeRegistration() => View(new Employee());

        // POST: EmployeeRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeRegistration(Employee model, IFormFile photo)
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
                    if (photo != null)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeePhoto");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await photo.CopyToAsync(fileStream);
                        }

                        model.Photo = $"/EmployeePhoto/{uniqueFileName}";
                    }

                    var success = await _httpClient.PostAsync("branchemployeeapi/employeeregistration", model);
                    if (success)
                        return RedirectToAction("Employee");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
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
                var employee = await _httpClient.GetAsync<Employee>($"branchemployeeapi/getbyid?id={id}");
                if (employee == null) return RedirectToAction("EP404", "EP");

                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: EmployeeUpdation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeUpdation(Employee model, IFormFile photo)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    if (photo != null)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "EmployeePhoto");
                        Directory.CreateDirectory(uploadsFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(photo.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await photo.CopyToAsync(fileStream);
                        }

                        model.Photo = $"/EmployeePhoto/{uniqueFileName}";
                    }

                    var success = await _httpClient.PutAsync("branchemployeeapi/employeeupdation", model);
                    if (success)
                        return RedirectToAction("Employee");
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
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
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
                var employee = await _httpClient.GetAsync<Employee>($"branchemployeeapi/getbyid?id={id}");
                if (employee == null) return RedirectToAction("EP404", "EP");

                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}