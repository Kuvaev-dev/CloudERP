using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.Models.FinancialModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CloudERP.Controllers.Company
{
    public class CompanyEmployeeController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public CompanyEmployeeController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: Employees
        [HttpGet]
        public async Task<ActionResult> Employees()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                return View(await _httpClient.GetAsync<IEnumerable<Employee>>($"companyemployeeapi/getall?companyId={_sessionHelper.CompanyID}"));
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
                ViewBag.Branches = await _httpClient.GetAsync<List<Domain.Models.Branch>>($"branchapi/getbycompany?companyId={_sessionHelper.CompanyID}");
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
        public async Task<ActionResult> EmployeeRegistration(Employee employee, IFormFile avatar)
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
                    using var content = new MultipartFormDataContent
                    {
                        { new StringContent(JsonConvert.SerializeObject(employee)), "model" }
                    };

                    if (avatar != null)
                    {
                        var stream = avatar.OpenReadStream();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(avatar.ContentType);
                        content.Add(fileContent, "file", avatar.FileName);
                    }

                    var success = await _httpClient.PostAsync("companyemployeeapi/employeeregistration", content);

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

                ViewBag.Branches = await _httpClient.GetAsync<List<Domain.Models.Branch>>($"branchapi/getbycompany?companyId={_sessionHelper.CompanyID}");
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
            var result = await _httpClient.PostAsync($"companyemployeeapi/processsalary?TIN={salary.TIN}", salary);
            return View(result ? salary : null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeSalaryConfirm(SalaryMV salaryMV)
        {
            salaryMV.TotalAmount = salaryMV.TransferAmount + (salaryMV.TransferAmount * (salaryMV.BonusPercentage ?? 0) / 100);

            var result = await _httpClient.PostAsync("companyemployeeapi/confirmsalary", salaryMV);
            if (result)
            {
                return RedirectToAction("PrintSalaryInvoice", new { id = salaryMV.EmployeeID });
            }
            return RedirectToAction("EmployeeSalary");
        }

        public async Task<ActionResult> SalaryHistory()
        {
            var history = await _httpClient.GetAsync<IEnumerable<Payroll>>($"companyemployeeapi/getsalaryhistory?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}");
            return View(history);
        }

        public async Task<ActionResult> PrintSalaryInvoice(int id)
        {
            var invoice = await _httpClient.GetAsync<Payroll>($"companyemployeeapi/getsalaryinvoice?id={id}");
            return View(invoice);
        }
    }
}