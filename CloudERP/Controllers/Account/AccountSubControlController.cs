using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
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
                if (subControls == null) return RedirectToAction("EP404", "EP");

                return View(subControls);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBag();

                return View(new AccountSubControl());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountSubControl model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.BranchID = _sessionHelper.BranchID;
                model.CompanyID = _sessionHelper.CompanyID;
                model.UserID = _sessionHelper.UserID;
                model.IsGlobal = false;

                await PopulateViewBag();

                if (model.AccountControlID > 0)
                {
                    var accountControl = await _accountControlRepository.GetByIdAsync(model.AccountControlID);
                    if (accountControl != null)
                    {
                        model.AccountHeadID = accountControl.AccountHeadID;
                    }
                }

                if (ModelState.IsValid)
                {
                    await _accountSubControlRepository.AddAsync(model);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
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

                await PopulateViewBag();

                return View(new AccountSubControl());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountSubControl model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.BranchID = _sessionHelper.BranchID;
                model.CompanyID = _sessionHelper.CompanyID;
                model.UserID = _sessionHelper.UserID;
                model.IsGlobal = false;

                await PopulateViewBag();

                if (model.AccountControlID > 0)
                {
                    var accountControl = await _accountControlRepository.GetByIdAsync(model.AccountControlID);
                    if (accountControl != null)
                    {
                        model.AccountHeadID = accountControl.AccountHeadID;
                    }
                }

                if (ModelState.IsValid)
                {
                    await _accountSubControlRepository.UpdateAsync(model);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBag()
        {
            ViewBag.AccountControlList = await _accountControlRepository.GetAllAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
        }
    }
}