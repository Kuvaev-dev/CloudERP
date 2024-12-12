using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Mapping;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class UserTypeController : Controller
    {
        private readonly IUserTypeService _service;
        private readonly IMapper<UserType, UserTypeMV> _mapper;

        public UserTypeController(IUserTypeService service, IMapper<UserType, UserTypeMV> mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            var userTypes = await _service.GetAllAsync();
            return View(userTypes);
        }

        // GET: UserType/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var userType = await _service.GetByIdAsync(id);
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
            return View(new UserTypeMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserTypeMV model)
        {
            if (ModelState.IsValid)
            {
                await _service.CreateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var userType = await _service.GetByIdAsync(id);
            if (userType == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(userType));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserTypeMV model)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}