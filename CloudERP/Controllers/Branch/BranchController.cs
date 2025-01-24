using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class BranchController : Controller
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IBranchTypeRepository _branchTypeRepository;
        private readonly SessionHelper _sessionHelper;

        private const int MAIN_BRANCH_TYPE_ID = 1;

        public BranchController(
            IBranchRepository branchRepository, 
            IBranchTypeRepository branchTypeRepository, 
            SessionHelper sessionHelper)
        {
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(IBranchRepository));
            _branchTypeRepository = branchTypeRepository ?? throw new ArgumentNullException(nameof(IBranchTypeRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Branch
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branches = _sessionHelper.BranchTypeID == MAIN_BRANCH_TYPE_ID
                ? await _branchRepository.GetByCompanyAsync(_sessionHelper.CompanyID)
                : await _branchRepository.GetSubBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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

                if (ModelState.IsValid)
                {
                    await _branchRepository.AddAsync(model);
                    return RedirectToAction("Index");
                }

                await PopulateViewBags(_sessionHelper.CompanyID);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Branch/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branch = await _branchRepository.GetByIdAsync(id);
                if (branch == null) return RedirectToAction("EP404", "EP");

                await PopulateViewBags(_sessionHelper.CompanyID, branch.ParentBranchID, branch.BranchTypeID);

                return View(branch);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
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
                    await _branchRepository.UpdateAsync(model);
                    return RedirectToAction("Index");
                }

                await PopulateViewBags(_sessionHelper.CompanyID, model.ParentBranchID, model.BranchTypeID);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> BranchesMap()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branches = await _branchRepository.GetSubBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
                return View(branches);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        private async Task PopulateViewBags(int companyID, int? selectedParentBranchID = null, int? selectedBranchTypeID = null)
        {
            var branches = await _branchRepository.GetByCompanyAsync(companyID);
            var branchTypes = await _branchTypeRepository.GetAllAsync();

            ViewBag.BrchID = new SelectList(branches, "BranchID", "BranchName", selectedParentBranchID);
            ViewBag.BranchTypeID = new SelectList(branchTypes, "BranchTypeID", "BranchTypeName", selectedBranchTypeID);
        }
    }
}
