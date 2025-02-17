using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;

namespace CloudERP.Controllers
{
    public class BranchController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        private const int MAIN_BRANCH_TYPE_ID = 1;

        public BranchController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Branch
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branches = await _httpClient.GetAsync<List<Branch>>(
                    $"branch/all/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}/{MAIN_BRANCH_TYPE_ID}");
                if (branches == null) return RedirectToAction("EP404", "EP");

                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Branch/Create
        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                await PopulateViewBags(_sessionHelper.CompanyID);
                return View(new Branch());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Branch model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BrchID = _sessionHelper.BranchID;

                if (ModelState.IsValid)
                {
                    await _httpClient.PostAsync("branch/create", model);
                    return RedirectToAction("Index");
                }

                await PopulateViewBags(_sessionHelper.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Branch/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branch = await _httpClient.GetAsync<Branch>($"branch/{id.Value}");
                if (branch == null) return RedirectToAction("EP404", "EP");

                return View(branch);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Branch model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;

                if (ModelState.IsValid)
                {
                    await _httpClient.PutAsync($"api/branch/update/{model.BranchID}", model);
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

        public async Task<ActionResult> BranchesMap()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branches = await _httpClient.GetAsync<List<Branch>>(
                    $"branch/sub-branches/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBags(int companyID, int? selectedParentBranchID = null, int? selectedBranchTypeID = null)
        {
            var branches = await _httpClient.GetAsync<List<Branch>>($"branch/{_sessionHelper.CompanyID}");
            var branchTypes = await _httpClient.GetAsync<List<BranchType>>("branch-type");

            ViewBag.BrchID = new SelectList(branches, "BranchID", "BranchName", selectedParentBranchID);
            ViewBag.BranchTypeID = new SelectList(branchTypes, "BranchTypeID", "BranchTypeName", selectedBranchTypeID);
        }
    }
}
