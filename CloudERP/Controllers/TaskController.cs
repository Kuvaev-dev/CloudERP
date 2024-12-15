using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class TaskController : Controller
    {
        private readonly ITaskService _service;
        private readonly IMapper<Domain.Models.TaskModel, TaskMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public TaskController(ITaskService service, IMapper<Domain.Models.TaskModel, TaskMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var tasks = await _service.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, _sessionHelper.UserID);
            return View(tasks);
        }

        public async Task<ActionResult> Details(int id)
        {
            var task = await _service.GetByIdAsync(id);
            if (task == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(task));
        }

        public ActionResult Create()
        {
            return View(new TaskMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(TaskMV model)
        {
            if (ModelState.IsValid)
            {
                var domainModel = _mapper.MapToDomain(model);
                await _service.CreateAsync(domainModel);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var task = await _service.GetByIdAsync(id);
            if (task == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(task));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(TaskMV model)
        {
            if (ModelState.IsValid)
            {
                var domainModel = _mapper.MapToDomain(model);
                await _service.UpdateAsync(domainModel);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Delete(int id)
        {
            var task = await _service.GetByIdAsync(id);
            if (task == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(task));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}