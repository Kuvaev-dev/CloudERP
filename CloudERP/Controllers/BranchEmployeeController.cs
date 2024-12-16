using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class BranchEmployeeController : Controller
    {
        private readonly IEmployeeService _service;
        private readonly IMapper<Employee, EmployeeMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public BranchEmployeeController(IEmployeeService service, IMapper<Employee, EmployeeMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        // GET: Employee
        public async Task<ActionResult> Employee()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var employees = await _service.GetEmployeesByBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

            return View(employees.Select(_mapper.MapToViewModel));
        }

        // GET: EmployeeRegistration
        public ActionResult EmployeeRegistration() => View(new EmployeeMV());

        // POST: EmployeeRegistration
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeRegistration(EmployeeMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;
            model.BranchID = _sessionHelper.BranchID;
            model.RegistrationDate = DateTime.Now;
            model.IsFirstLogin = true;

            if (ModelState.IsValid)
            {
                await _service.CreateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Employee");
            }

            return View(model);
        }

        // GET: EmployeeUpdation
        public async Task<ActionResult> EmployeeUpdation(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var employee = await _service.GetByIdAsync(id);
            if (employee == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(employee));
        }

        // POST: EmployeeUpdation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeUpdation(EmployeeMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Employee");
            }

            return View(model);
        }

        // GET: ViewProfile
        public async Task<ActionResult> ViewProfile(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var employee = await _service.GetByIdAsync(id);
            if (employee == null) return RedirectToAction("EP404", "EP");

            var viewModel = _mapper.MapToViewModel(employee);
            return View(viewModel);
        }
    }
}