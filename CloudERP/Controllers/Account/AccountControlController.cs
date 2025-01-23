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
    public class AccountControlController : Controller
    {
        private readonly IAccountControlRepository _accountControlRepository;
        private readonly IAccountHeadRepository _accountHeadRepository;
        private readonly SessionHelper _sessionHelper;

        public AccountControlController(
            IAccountControlRepository accountControlRepository, 
            IAccountHeadRepository accountHeadRepository, 
            SessionHelper sessionHelper)
        {
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
                var accountControls = await _accountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                return View(accountControls);
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
                return View(new AccountControlMV
                {
                    AccountHeadList = await GetAccountHeadList()
                });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountControlMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.AccountControl.BranchID = _sessionHelper.BranchID;
                model.AccountControl.CompanyID = _sessionHelper.CompanyID;
                model.AccountControl.UserID = _sessionHelper.UserID;
                model.AccountHeadList = await GetAccountHeadList();

                if (ModelState.IsValid)
                {
                    await _accountControlRepository.AddAsync(model.AccountControl);
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

                var accountHeads = await _accountHeadRepository.GetAllAsync();
                if (accountHeads == null) return HttpNotFound();

                var accountControl = await _accountControlRepository.GetByIdAsync(id.Value);
                if (accountControl == null) return HttpNotFound();

                AccountControlMV accountControlMV = new AccountControlMV()
                {
                    AccountControl = accountControl,
                    AccountHeadList = accountHeads.Select(ah => new SelectListItem
                    {
                        Value = ah.AccountHeadID.ToString(),
                        Text = ah.AccountHeadName,
                        Selected = ah.AccountHeadID == accountControl.AccountHeadID
                    }).ToList()
                };

                return View(accountControlMV);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountControlMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {   
                    await _accountControlRepository.UpdateAsync(model.AccountControl);
                    return RedirectToAction("Index");
                }

                model.AccountHeadList = await GetAccountHeadList();

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<IEnumerable<SelectListItem>> GetAccountHeadList()
        {
            var accountHeads = await _accountHeadRepository.GetAllAsync();
            return accountHeads
                .Select(ah => new SelectListItem
                {
                    Value = ah.AccountHeadID.ToString(),
                    Text = ah.AccountHeadName
                })
                .ToList();
        }
    }
}