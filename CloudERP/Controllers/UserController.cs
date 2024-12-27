using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly SessionHelper _sessionHelper;

        public UserController(IUserRepository userRepository, IUserTypeRepository userTypeRepository, SessionHelper sessionHelper)
        {
            _userRepository = userRepository;
            _userTypeRepository = userTypeRepository;
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
            ViewBag.UserTypeID = new SelectList(await _userTypeRepository.GetAllAsync(), "UserTypeID", "UserType");
            return View(new User());
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