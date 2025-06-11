using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.Utilities.TaskList
{
    public class TaskController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public TaskController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var tasks = await _httpClient.GetAsync<IEnumerable<TaskModel>>(
                    $"taskapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&userId={_sessionHelper.UserID}");
                return View(tasks ?? []);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _httpClient.GetAsync<TaskModel>($"taskapi/getbyid?id={id}");
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                model.AssignedByUserID = 0;
                model.AssignedToUserID = 0;
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                var success = await _httpClient.PostAsync("taskapi/create", model);
                if (success) return RedirectToAction("Index");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AssignTask()
        {
            var employees = await _httpClient.GetAsync<IEnumerable<Employee>>(
                $"taskapi/getemployeesfortaskassignment?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
            ViewBag.Employees = employees;
            return View(new TaskModel() { DueDate = DateTime.Now });
        }

        [HttpPost]
        public async Task<ActionResult> AssignTask(TaskModel model)
        {
            model.IsCompleted = false;
            model.AssignedByUserID = _sessionHelper.UserID;
            model.CompanyID = _sessionHelper.CompanyID;
            model.UserID = model.AssignedToUserID ?? 0;

            if (ModelState.IsValid)
            {
                await _httpClient.PostAsync("taskapi/create", model);
                return RedirectToAction("AssignTask");
            }

            var employees = await _httpClient.GetAsync<IEnumerable<Employee>>(
                $"taskapi/getemployeesfortaskassignment?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
            ViewBag.Employees = employees;

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _httpClient.GetAsync<TaskModel>($"taskapi/getbyid?id={id}");
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                    await _httpClient.PutAsync($"taskapi/update?id={model.TaskID}", model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Complete(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _httpClient.GetAsync<TaskModel>($"taskapi/getbyid?id={id}");
                if (task == null) return RedirectToAction("EP404", "EP");
                task.IsCompleted = true;

                await _httpClient.PutAsync($"taskapi/update?id={task.TaskID}", task);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _httpClient.GetAsync<TaskModel>($"taskapi/getbyid?id={id}");
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                await _httpClient.DeleteAsync($"taskapi/delete?id={id}");
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}