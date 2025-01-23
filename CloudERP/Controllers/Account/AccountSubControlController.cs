using System;
using System.Collections.Generic;
using System.Linq;
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

        public AccountSubControlController(
            IAccountSubControlRepository accountSubControlRepository, 
            IAccountControlRepository accountControlRepository, 
            IAccountHeadRepository accountHeadRepository, 
            SessionHelper sessionHelper)
        {
            _accountSubControlRepository = accountSubControlRepository ?? throw new ArgumentNullException(nameof(IAccountSubControlRepository));
            _accountControlRepository = accountControlRepository ?? throw new ArgumentNullException(nameof(IAccountControlRepository));
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(IAccountHeadRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try 
            { 
                var subControls = await _accountSubControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                return View(subControls);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var model = new AccountSubControlMV
                {
                    AccountControlList = await GetAccountControlList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSubControlMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.AccountSubControl.BranchID = _sessionHelper.BranchID;
                model.AccountSubControl.CompanyID = _sessionHelper.CompanyID;
                model.AccountSubControl.UserID = _sessionHelper.UserID;
                model.AccountControlList = await GetAccountControlList();

                if (model.AccountSubControl.AccountControlID > 0)
                {
                    var accountControl = await _accountControlRepository.GetByIdAsync(model.AccountSubControl.AccountControlID);
                    if (accountControl != null)
                    {
                        model.AccountSubControl.AccountHeadID = accountControl.AccountHeadID;
                    }
                }

                if (ModelState.IsValid)
                {
                    await _accountSubControlRepository.AddAsync(model.AccountSubControl);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null) return RedirectToAction("Index");

                var accountControls = await _accountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                if (accountControls == null) return RedirectToAction("EP404", "EP");

                var subControl = await _accountSubControlRepository.GetByIdAsync(id.Value);
                if (subControl == null) return RedirectToAction("EP404", "EP");

                AccountSubControlMV accountSubControlMV = new AccountSubControlMV
                {
                    AccountSubControl = subControl,
                    AccountControlList = await GetAccountControlList()
                };

                return View(accountSubControlMV);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSubControlMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.AccountSubControl.BranchID = _sessionHelper.BranchID;
                model.AccountSubControl.CompanyID = _sessionHelper.CompanyID;
                model.AccountSubControl.UserID = _sessionHelper.UserID;
                model.AccountControlList = await GetAccountControlList();

                if (model.AccountSubControl.AccountControlID > 0)
                {
                    var accountControl = await _accountControlRepository.GetByIdAsync(model.AccountSubControl.AccountControlID);
                    if (accountControl != null)
                    {
                        model.AccountSubControl.AccountHeadID = accountControl.AccountHeadID;
                    }
                }

                if (ModelState.IsValid)
                {
                    await _accountSubControlRepository.UpdateAsync(model.AccountSubControl);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetAccountControlList()
        {
            var accountControls = await _accountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return accountControls
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountControlID.ToString(),
                    Text = ah.AccountControlName
                })
                .ToList();
        }
    }
}