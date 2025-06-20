﻿using CloudERP.Models;
using Domain.Models;
using Domain.UtilsAccess;
using Localization.CloudERP.Messages;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Company
{
    public class CompanyEmployeeController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;
        private readonly IImageUploadHelper _imageUploadHelper;

        private const string EMPLOYEE_PHOTO_FOLDER = "EmployeePhoto";
        private const string DEFAULT_EMPLOYEE_AVATAR_PATH = "~/EmployeePhoto/Default/default.png";

        public CompanyEmployeeController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IImageUploadHelper imageUploadHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _imageUploadHelper = imageUploadHelper ?? throw new ArgumentNullException(nameof(imageUploadHelper));
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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

                if (avatar != null) 
                    employee.Photo = await _imageUploadHelper.UploadImageAsync(avatar, EMPLOYEE_PHOTO_FOLDER);
                else
                    employee.Photo = DEFAULT_EMPLOYEE_AVATAR_PATH;

                if (ModelState.ContainsKey("avatar"))
                    ModelState.Remove("avatar");

                if (ModelState.IsValid)
                {
                    var response = await _httpClient.PostAsync("companyemployeeapi/employeeregistration", employee);
                    if (response) return RedirectToAction("Employees");
                    else ViewBag.ErrorMessage = Messages.AlreadyExists;
                }

                ViewBag.Branches = await _httpClient.GetAsync<List<Domain.Models.Branch>>($"branchapi/getbycompany?companyId={_sessionHelper.CompanyID}");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
        public async Task<ActionResult> EmployeeSalary(string TIN)
        {
            var salary = await _httpClient.GetAsync<SalaryMV>($"companyemployeeapi/processsalary?TIN={TIN}");
            return View(salary);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeSalaryConfirm(SalaryMV salaryMV)
        {
            salaryMV.CompanyID = _sessionHelper.CompanyID;
            salaryMV.BranchID = _sessionHelper.BranchID;
            salaryMV.UserID = _sessionHelper.UserID;

            var response = await _httpClient.PostAsync("companyemployeeapi/confirmsalary", salaryMV);
            if (response)
            {
                return RedirectToAction("PrintSalaryInvoice");
            }

            return RedirectToAction("EmployeeSalary");
        }

        public async Task<ActionResult> SalaryHistory()
        {
            var history = await _httpClient.GetAsync<IEnumerable<Payroll>>($"companyemployeeapi/getsalaryhistory?branchId={_sessionHelper.BranchID}&companyId={_sessionHelper.CompanyID}");
            return View(history);
        }

        public async Task<ActionResult> PrintSalaryInvoice()
        {
            var invoice = await _httpClient.GetAsync<Payroll>($"companyemployeeapi/getlatestsalaryinvoice");
            return View(invoice);
        }
    }
}