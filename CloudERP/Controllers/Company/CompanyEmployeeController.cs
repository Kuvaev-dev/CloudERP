using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CompanyEmployeeController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;
        
        public CompanyEmployeeController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Employees
        [HttpGet]
        public async Task<ActionResult> Employees()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _httpClient.GetAsync<List<Employee>>("company-employee/employees"));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> EmployeeRegistration()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                ViewBag.Branches = await _httpClient.GetAsync<List<Branch>>($"branch/{_sessionHelper.CompanyID}");
                return View(new Employee());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeRegistration(Employee employee, HttpPostedFileBase avatar)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                employee.CompanyID = _sessionHelper.CompanyID;
                employee.UserID = null;
                employee.RegistrationDate = DateTime.Now;
                employee.IsFirstLogin = true;

                if (ModelState.IsValid)
                {
                    using (var content = new MultipartFormDataContent())
                    {
                        content.Add(new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(employee)), "model");

                        if (avatar != null)
                        {
                            var stream = avatar.InputStream;
                            var fileContent = new StreamContent(stream);
                            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(avatar.ContentType);
                            content.Add(fileContent, "file", avatar.FileName);
                        }

                        await _httpClient.PostAsync("company-employee/registration", content);
                    }

                    return RedirectToAction("Employees");
                }

                ViewBag.Branches = await _httpClient.GetAsync<List<Branch>>($"branch/{_sessionHelper.CompanyID}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }

            return View(employee);
        }

        public ActionResult EmployeeSalary()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var salary = new SalaryMV
            {
                SalaryMonth = DateTime.Now.AddMonths(-1).ToString("MMMM"),
                SalaryYear = DateTime.Now.AddMonths(-1).ToString("yyyy")
            };

            return View(salary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeSalary(SalaryMV salary)
        {
            var result = await _httpClient.PostAsync("salary/process", salary);
            return View(result ? salary : null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeSalaryConfirm(SalaryMV salaryMV)
        {
            var result = await _httpClient.PostAsync("salary/confirm", salaryMV);
            if (result)
            {
                return RedirectToAction("PrintSalaryInvoice", new { id = salaryMV.EmployeeID });
            }
            return RedirectToAction("EmployeeSalary");
        }

        public async Task<ActionResult> SalaryHistory()
        {
            var history = await _httpClient.GetAsync<List<Salary>>("salary/history");
            return View(history);
        }

        public async Task<ActionResult> PrintSalaryInvoice(int id)
        {
            var invoice = await _httpClient.GetAsync<Salary>($"salary/invoice/{id}");
            return View(invoice);
        }
    }
}