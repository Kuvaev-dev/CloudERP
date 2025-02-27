using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.Branch
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
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: Branch
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branches = await _httpClient.GetAsync<IEnumerable<Domain.Models.Branch>>(
                    $"branchapi/getall?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&mainBranchTypeID={MAIN_BRANCH_TYPE_ID}");
                if (branches == null) return RedirectToAction("EP404", "EP");

                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SubBranchs()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branches = await _httpClient.GetAsync<IEnumerable<Domain.Models.Branch>>(
                    $"branchapi/getsubbranches?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}&mainBranchTypeID={MAIN_BRANCH_TYPE_ID}");
                if (branches == null) return RedirectToAction("EP404", "EP");

                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
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
                return View(new Domain.Models.Branch());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Domain.Models.Branch model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BrchID = _sessionHelper.BranchID;

                if (ModelState.IsValid)
                {
                    await _httpClient.PostAsync("branchapi/create", model);
                    return RedirectToAction("Index");
                }

                await PopulateViewBags(_sessionHelper.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
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
                var branch = await _httpClient.GetAsync<Domain.Models.Branch>($"branchapi/getbyid?id={id}");
                if (branch == null) return RedirectToAction("EP404", "EP");

                return View(branch);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Domain.Models.Branch model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;

                if (ModelState.IsValid)
                {
                    await _httpClient.PutAsync($"branchapi/update?id={model.BranchID}", model);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> BranchesMap()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branches = await _httpClient.GetAsync<IEnumerable<Domain.Models.Branch>>(
                    $"branchapi/getsubbranches?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBags(int companyID, int? selectedParentBranchID = null, int? selectedBranchTypeID = null)
        {
            var branches = await _httpClient.GetAsync<IEnumerable<Domain.Models.Branch>>($"branchapi/getbycompany?companyId={_sessionHelper.CompanyID}");
            var branchTypes = await _httpClient.GetAsync<IEnumerable<BranchType>>("branchtypeapi/getall");

            ViewBag.BrchID = new SelectList(branches, "BranchID", "BranchName", selectedParentBranchID);
            ViewBag.BranchTypeID = new SelectList(branchTypes, "BranchTypeID", "BranchTypeName", selectedBranchTypeID);
        }
    }
}
