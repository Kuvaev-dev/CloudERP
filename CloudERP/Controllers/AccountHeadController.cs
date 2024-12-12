using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class AccountHeadController : Controller
    {
        private readonly IAccountHeadService _service;
        private readonly IMapper<AccountHead, AccountHeadMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public AccountHeadController(IAccountHeadService service, IMapper<AccountHead, AccountHeadMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var accountHeads = await _service.GetAllAsync();
            return View(accountHeads);
        }

        public ActionResult Create()
        {
            return View(new AccountHeadMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountHeadMV model)
        {
            if (ModelState.IsValid)
            {
                model.UserID = _sessionHelper.UserID;
                await _service.CreateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var accountHead = await _service.GetByIdAsync(id);
            if (accountHead == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(accountHead));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountHeadMV model)
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