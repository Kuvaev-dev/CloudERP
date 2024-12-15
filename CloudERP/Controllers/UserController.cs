using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _service;
        private readonly IUserTypeService _userTypeService;
        private readonly IMapper<Domain.Models.User, UserMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public UserController(IUserService service, IUserTypeService userTypeService, IMapper<Domain.Models.User, UserMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _userTypeService = userTypeService;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var users = await _service.GetAllAsync();
            return View(users);
        }

        public async Task<ActionResult> SubBranchUser()
        {
            var users = await _service.GetByBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchTypeID, _sessionHelper.BranchID);
            return View(users);
        }

        public async Task<ActionResult> Details(int id)
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(user));
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.UserTypeID = new SelectList(await _userTypeService.GetAllAsync(), "UserTypeID", "UserType");
            return View(new UserMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserMV model)
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
            var user = await _service.GetByIdAsync(id);
            if (user == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(user));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserMV model)
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
            var user = await _service.GetByIdAsync(id);
            if (user == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(user));
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