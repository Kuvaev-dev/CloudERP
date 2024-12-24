using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class AccountSubControlController : Controller
    {
        private readonly IAccountSubControlRepository _accountSubControlRepository;
        private readonly IAccountControlRepository _accountControlRepository;
        private readonly IAccountHeadRepository _accountHeadRepository;
        private readonly SessionHelper _sessionHelper;

        public AccountSubControlController(IAccountSubControlRepository accountSubControlRepository, IAccountControlRepository accountControlRepository, IAccountHeadRepository accountHeadRepository, SessionHelper sessionHelper)
        {
            _accountSubControlRepository = accountSubControlRepository;
            _accountControlRepository = accountControlRepository;
            _accountHeadRepository = accountHeadRepository;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            var subControls = await _accountSubControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(subControls);
        }

        public async Task<ActionResult> Create()
        {
            var model = new AccountSubControlMV
            {
                AccountControlList = await GetAccountControlList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSubControlMV model)
        {
            var accountHead = await _accountHeadRepository.GetByIdAsync(model.AccountSubControl.AccountControlID);

            model.AccountSubControl.BranchID = _sessionHelper.BranchID;
            model.AccountSubControl.CompanyID = _sessionHelper.CompanyID;
            model.AccountSubControl.UserID = _sessionHelper.UserID;
            model.AccountControlList = null;
            model.AccountSubControl.AccountHeadID = accountHead.AccountHeadID;

            if (ModelState.IsValid)
            {
                await _accountSubControlRepository.AddAsync(model.AccountSubControl);
                return RedirectToAction("Index");
            }

            model.AccountControlList = await GetAccountControlList();

            return View(model);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null) return RedirectToAction("Index");

            var accountControls = await _accountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            if (accountControls == null || !accountControls.Any())
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "No Account Controls found.");

            var subControl = await _accountSubControlRepository.GetByIdAsync(id.Value);
            if (subControl == null) return HttpNotFound();

            AccountSubControlMV accountSubControlMV = new AccountSubControlMV
            {
                AccountSubControl = subControl,
                AccountControlList = await GetAccountControlList()
            };

            return View(accountSubControlMV);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSubControlMV model)
        {
            if (ModelState.IsValid)
            {
                await _accountSubControlRepository.UpdateAsync(model.AccountSubControl);
                return RedirectToAction("Index");
            }

            model.AccountControlList = await GetAccountControlList();

            return View(model);
        }

        public async Task<IEnumerable<SelectListItem>> GetAccountControlList()
        {
            var accountControls = await _accountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return accountControls
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName
                })
                .ToList();
        }
    }
}