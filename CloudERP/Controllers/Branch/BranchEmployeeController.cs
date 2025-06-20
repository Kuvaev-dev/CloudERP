using Domain.Models;
using Domain.UtilsAccess;
using Localization.CloudERP.Messages;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Branch
{
    public class BranchEmployeeController : Controller
    {
        private readonly ISessionHelper _sessionHelper;
        private readonly IHttpClientHelper _httpClient;
        private readonly IImageUploadHelper _imageUploadHelper;
        private readonly IPhoneNumberHelper _phoneNumberHelper;

        private const string EMPLOYEE_PHOTO_FOLDER = "EmployeePhoto";
        private const string DEFAULT_EMPLOYEE_AVATAR_PATH = "~/EmployeePhoto/Default/default.png";

        public BranchEmployeeController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IImageUploadHelper imageUploadHelper,
            IPhoneNumberHelper phoneNumberHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _imageUploadHelper = imageUploadHelper ?? throw new ArgumentNullException(nameof(imageUploadHelper));
            _phoneNumberHelper = phoneNumberHelper ?? throw new ArgumentNullException(nameof(phoneNumberHelper));
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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

                if (logo != null)
                    model.Photo = await _imageUploadHelper.UploadImageAsync(logo, EMPLOYEE_PHOTO_FOLDER);
                else
                    model.Photo = DEFAULT_EMPLOYEE_AVATAR_PATH;

                if (ModelState.ContainsKey("logo"))
                    ModelState.Remove("logo");

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PostAsync("branchemployeeapi/employeeregistration", model);
                    if (success) return RedirectToAction("Employee");
                    else ViewBag.ErrorMessage = Messages.AlreadyExists;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                employee.ContactNumber = _phoneNumberHelper.ExtractNationalNumber(employee.ContactNumber);
                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                if (logo != null)
                    model.Photo = await _imageUploadHelper.UploadImageAsync(logo, EMPLOYEE_PHOTO_FOLDER);
                else
                    model.Photo = DEFAULT_EMPLOYEE_AVATAR_PATH;

                if (ModelState.ContainsKey("logo"))
                    ModelState.Remove("logo");

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PutAsync("branchemployeeapi/employeeupdation", model);
                    if (success) return RedirectToAction("Employee");
                    else ViewBag.ErrorMessage = Messages.AlreadyExists;
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}