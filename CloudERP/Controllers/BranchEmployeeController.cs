using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BranchEmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly SessionHelper _sessionHelper;

        public BranchEmployeeController(IEmployeeRepository employeeRepository, SessionHelper sessionHelper)
        {
            _employeeRepository = employeeRepository;
            _sessionHelper = sessionHelper;
        }

        // GET: Employee
        public async Task<ActionResult> Employee()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var employees = await _employeeRepository.GetByBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

            return View(employees);
        }

        // GET: EmployeeRegistration
        public ActionResult EmployeeRegistration() => View(new Employee());

        // POST: EmployeeRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeRegistration(Employee model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;
            model.BranchID = _sessionHelper.BranchID;
            model.RegistrationDate = DateTime.Now;
            model.IsFirstLogin = true;

            if (ModelState.IsValid)
            {
                await _employeeRepository.AddAsync(model);
                return RedirectToAction("Employee");
            }

            return View(model);
        }

        // GET: EmployeeUpdation
        public async Task<ActionResult> EmployeeUpdation(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return HttpNotFound();

            return View(employee);
        }

        // POST: EmployeeUpdation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeUpdation(Employee model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                await _employeeRepository.UpdateAsync(model);
                return RedirectToAction("Employee");
            }

            return View(model);
        }

        // GET: ViewProfile
        public async Task<ActionResult> ViewProfile(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var employee = await _employeeRepository.GetByIdAsync(id);
            if (employee == null) return RedirectToAction("EP404", "EP");

            return View(employee);
        }
    }
}