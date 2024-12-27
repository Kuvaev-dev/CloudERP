using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class UserTypeController : Controller
    {
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly SessionHelper _sessionHelper;

        public UserTypeController(IUserTypeRepository userTypeRepository, SessionHelper sessionHelper)
        {
            _userTypeRepository = userTypeRepository;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var userTypes = await _userTypeRepository.GetAllAsync();
            return View(userTypes);
        }

        // GET: UserType/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                if (!_sessionHelper.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Home");
                }

                var userType = await _userTypeRepository.GetByIdAsync(id);
                return View(userType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult Create()
        {
            return View(new UserType());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserType model)
        {
            if (ModelState.IsValid)
            {
                await _userTypeRepository.AddAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var userType = await _userTypeRepository.GetByIdAsync(id);
            if (userType == null) return HttpNotFound();

            return View(userType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserType model)
        {
            if (ModelState.IsValid)
            {
                await _userTypeRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}