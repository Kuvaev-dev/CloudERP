using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Utilities.TaskList
{
    public class TaskController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public TaskController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var tasks = await _httpClient.GetAsync<List<AccountHead>>($"task/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}/{_sessionHelper.UserID}");
                if (tasks == null) return RedirectToAction("EP404", "EP");

                return View(tasks);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Details(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _httpClient.GetAsync<TaskModel>($"task/{id}");
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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

                if (ModelState.IsValid)
                {
                    await _httpClient.PostAsync<TaskModel>("task/create", model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AssignTask()
        {
            var employees = await _httpClient.GetAsync<List<Employee>>($"task/employees/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
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
                await _httpClient.PostAsync<TaskModel>("task/create", model);
                return RedirectToAction("AssignTask");
            }

            var employees = await _httpClient.GetAsync<List<Employee>>($"task/employees/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
            ViewBag.Employees = employees;

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _httpClient.GetAsync<TaskModel>($"task/{id}");
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                    await _httpClient.PutAsync<TaskModel>($"task/update/{model.TaskID}", model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Complete(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _httpClient.GetAsync<TaskModel>($"task/{id}");
                if (task == null) return RedirectToAction("EP404", "EP");
                task.IsCompleted = true;

                await _httpClient.PutAsync<TaskModel>($"account-head/update/{task.TaskID}", task);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var task = await _httpClient.GetAsync<TaskModel>($"task/{id}");
                if (task == null) return RedirectToAction("EP404", "EP");

                return View(task);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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
                await _httpClient.PostAsync<TaskModel>("account-head/delete", id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}