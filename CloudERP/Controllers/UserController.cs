using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeService _userTypeService;
        private readonly SessionHelper _sessionHelper;

        public UserController(IUserRepository userRepository, IUserTypeService userTypeService, SessionHelper sessionHelper)
        {
            _userRepository = userRepository;
            _userTypeService = userTypeService;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var users = await _userRepository.GetAllAsync();
            return View(users);
        }

        public async Task<ActionResult> SubBranchUser()
        {
            var users = await _userRepository.GetByBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchTypeID, _sessionHelper.BranchID);
            return View(users);
        }

        public async Task<ActionResult> Details(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.UserTypeID = new SelectList(await _userTypeService.GetAllAsync(), "UserTypeID", "UserType");
            return View(new UserMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User model)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.AddAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return HttpNotFound();

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User model)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}