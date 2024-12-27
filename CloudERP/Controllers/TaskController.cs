using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskRepository _taskRepository;
        private readonly SessionHelper _sessionHelper;

        public TaskController(ITaskRepository taskRepository, SessionHelper sessionHelper)
        {
            _taskRepository = taskRepository;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var tasks = await _taskRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID);
            return View(tasks);
        }

        public async Task<ActionResult> Details(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return HttpNotFound();

            return View(task);
        }

        public ActionResult Create()
        {
            return View(new TaskModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TaskModel model)
        {
            if (ModelState.IsValid)
            {
                await _taskRepository.AddAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return HttpNotFound();

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TaskModel model)
        {
            if (ModelState.IsValid)
            {
                await _taskRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var task = await _taskRepository.GetByIdAsync(id);
            if (task == null) return HttpNotFound();

            return View(task);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _taskRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}