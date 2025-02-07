using CloudERP.Helpers;
using DatabaseAccess.Factories;
using Domain.Interfaces;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BranchEmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFileService _fileService;
        private readonly IFileAdapterFactory _fileAdapterFactory;
        private readonly SessionHelper _sessionHelper;

        private const string EMPLOYEE_AVATAR_PATH = "~/Content/EmployeePhoto";
        private const string DEFAULT_EMPLOYEE_AVATAR_PATH = "~/Content/EmployeePhoto/Default/default.png";

        public BranchEmployeeController(
            IEmployeeRepository employeeRepository,
            IFileService fileService,
            IFileAdapterFactory fileAdapterFactory,
            SessionHelper sessionHelper)
        {
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(IFileService));
            _fileAdapterFactory = fileAdapterFactory ?? throw new ArgumentNullException(nameof(IFileAdapterFactory));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Employee
        public async Task<ActionResult> Employee()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var employees = await _employeeRepository.GetByBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(employees);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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

                if (logo != null)
                {
                    var fileName = $"{model.UserID}.jpg";

                    var fileAdapter = _fileAdapterFactory.Create(logo);
                    model.Photo = _fileService.UploadPhoto(fileAdapter, EMPLOYEE_AVATAR_PATH, fileName);
                }
                else
                {
                    model.Photo = _fileService.SetDefaultPhotoPath(DEFAULT_EMPLOYEE_AVATAR_PATH);
                }

                if (ModelState.IsValid)
                {
                    await _employeeRepository.AddAsync(model);
                    return RedirectToAction("Employee");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null) return RedirectToAction("EP404", "EP");

                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                    if (logo != null)
                    {
                        var fileName = $"{model.UserID}.jpg";

                        var fileAdapter = _fileAdapterFactory.Create(logo);
                        model.Photo = _fileService.UploadPhoto(fileAdapter, EMPLOYEE_AVATAR_PATH, fileName);
                    }
                    else
                    {
                        model.Photo = _fileService.SetDefaultPhotoPath(DEFAULT_EMPLOYEE_AVATAR_PATH);
                    }

                    await _employeeRepository.UpdateAsync(model);
                    return RedirectToAction("Employee");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null) return RedirectToAction("EP404", "EP");

                return View(employee);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}