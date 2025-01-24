using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess.Repositories;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly SessionHelper _sessionHelper;

        public TaskController(
            ITaskRepository taskRepository, 
            SessionHelper sessionHelper,
            IEmployeeRepository employeeRepository)
        {
            _taskRepository = taskRepository ?? throw new ArgumentNullException(nameof(ITaskRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _employeeRepository = employeeRepository;
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var tasks = await _taskRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID);
                return View(tasks);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new TaskModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TaskModel model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    await _taskRepository.AddAsync(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AssignTask(int branchId)
        {
            var employees = await _employeeRepository.GetEmployeesForTaskAssignmentAsync(branchId);
            var model = new TaskAssignmentVM
            {
                AvailableEmployees = employees,
                BranchID = branchId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AssignTask(TaskModel taskModel)
        {
            if (ModelState.IsValid)
            {
                await _taskRepository.AddAsync(taskModel);
                return View();
            }

            var employees = await _employeeRepository.GetEmployeesForTaskAssignmentAsync(taskModel.BranchID);
            var model = new TaskAssignmentVM
            {
                AvailableEmployees = employees,
                TaskModel = taskModel,
                BranchID = taskModel.BranchID
            };

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TaskModel model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _taskRepository.UpdateAsync(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Complete(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null) return RedirectToAction("EP404", "EP");
                task.IsCompleted = true;

                await _taskRepository.UpdateAsync(task);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _taskRepository.GetByIdAsync(id);
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await _taskRepository.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}