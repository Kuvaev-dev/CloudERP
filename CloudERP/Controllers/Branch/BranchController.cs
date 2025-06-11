using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.Branch
{
    public class BranchController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;
        private readonly IPhoneNumberHelper _phoneNumberHelper;

        private const int MAIN_BRANCH_TYPE_ID = 1;

        public BranchController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IPhoneNumberHelper phoneNumberHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _phoneNumberHelper = phoneNumberHelper ?? throw new ArgumentNullException(nameof(phoneNumberHelper));
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
                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BrchID = _sessionHelper.BranchID;

                var success = await _httpClient.PostAsync("branchapi/create", model);
                if (success) return RedirectToAction("Index");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                await PopulateViewBags(_sessionHelper.CompanyID);

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                var branch = await _httpClient.GetAsync<Domain.Models.Branch>($"branchapi/getbyid?id={id.Value}");
                if (branch == null) return RedirectToAction("EP404", "EP");
                branch.BranchContact = _phoneNumberHelper.ExtractNationalNumber(branch.BranchContact);

                return View(branch);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;

                var success = await _httpClient.PutAsync($"branchapi/update?id={model.BranchID}", model);
                if (success) return RedirectToAction("Index");
                else ViewBag.ErrorMessage = Messages.AlreadyExists;

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
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
