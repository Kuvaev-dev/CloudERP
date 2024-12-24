using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class AccountHeadController : Controller
    {
        private readonly IAccountHeadRepository _accountHeadRepository;
        private readonly SessionHelper _sessionHelper;

        public AccountHeadController(IAccountHeadRepository accountHeadRepository, SessionHelper sessionHelper)
        {
            _accountHeadRepository = accountHeadRepository;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var accountHeads = await _accountHeadRepository.GetAllAsync();
            return View(accountHeads);
        }

        public ActionResult Create()
        {
            return View(new AccountHead());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountHead model)
        {
            if (ModelState.IsValid)
            {
                model.UserID = _sessionHelper.UserID;
                await _accountHeadRepository.AddAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var accountHead = await _accountHeadRepository.GetByIdAsync(id);
            if (accountHead == null) return HttpNotFound();

            return View(accountHead);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountHead model)
        {
            if (ModelState.IsValid)
            {
                await _accountHeadRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}