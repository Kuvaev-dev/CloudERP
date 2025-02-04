using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CloudERP.Helpers;
using DatabaseAccess;
using DatabaseAccess.Repositories;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers.Account
{
    public class AccountActivityController : Controller
    {
        private readonly IAccountActivityRepository _accountActivityRepository;
        private readonly SessionHelper _sessionHelper;

        public AccountActivityController(
            IAccountActivityRepository accountActivityRepository,
            SessionHelper sessionHelper)
        {
            _accountActivityRepository = accountActivityRepository;
            _sessionHelper = sessionHelper;
        }

        // GET: AccountActivity
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var accountActivities = await _accountActivityRepository.GetAllAsync();
                if (accountActivities == null) return RedirectToAction("EP404", "EP");

                return View(accountActivities);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: AccountActivity/Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new AccountActivity());
        }

        // POST: AccountActivity/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountActivity model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _accountActivityRepository.AddAsync(model);
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

        // GET: AccountActivity/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var accountActivity = await _accountActivityRepository.GetByIdAsync(id.Value);
                if (accountActivity == null) return RedirectToAction("EP404", "EP");

                return View(accountActivity);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: AccountActivity/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountActivity model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _accountActivityRepository.UpdateAsync(model);
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
    }
}
