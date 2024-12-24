using System.Linq;
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

        public BranchController(IBranchRepository branchRepository, IBranchTypeRepository branchTypeRepository, SessionHelper sessionHelper)
        {
            _branchRepository = branchRepository;
            _branchTypeRepository = branchTypeRepository;
            _sessionHelper = sessionHelper;
        }

        // GET: Branch
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var branches = _sessionHelper.BranchTypeID == 1
                ? await _branchRepository.GetByCompanyAsync(_sessionHelper.CompanyID)
                : await _branchRepository.GetSubBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);

            return View(branches);
        }

        // GET: Branch/Create
        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            await PopulateViewBags(_sessionHelper.CompanyID);

            return View(new Branch());
        }

        private async Task<bool> CheckBranchExistsAsync(Branch branch, bool isEdit = false)
        {
            var branches = await _branchRepository.GetByCompanyAsync(branch.CompanyID);
            return branches.Any(b =>
                b.BranchName == branch.BranchName ||
                b.BranchContact == branch.BranchContact ||
                b.BranchAddress == branch.BranchAddress &&
                (!isEdit || b.BranchID != branch.BranchID));
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Branch model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;

            if (ModelState.IsValid)
            {
                if (await CheckBranchExistsAsync(model))
                {
                    ModelState.AddModelError("", Resources.Messages.BranchEists);
                    await PopulateViewBags(_sessionHelper.CompanyID);
                    return View(model);
                }

                await _branchRepository.AddAsync(model);
                return RedirectToAction("Index");
            }

            await PopulateViewBags(_sessionHelper.CompanyID);
            return View(model);
        }

        // GET: Branch/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var branch = await _branchRepository.GetByIdAsync(id);
            if (branch == null) return HttpNotFound();

            await PopulateViewBags(_sessionHelper.CompanyID, branch.ParentBranchID, branch.BranchTypeID);

            return View(branch);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Branch model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;

            if (ModelState.IsValid)
            {
                if (await CheckBranchExistsAsync(model, isEdit: true))
                {
                    ModelState.AddModelError("", Resources.Messages.BranchEists);
                    await PopulateViewBags(_sessionHelper.CompanyID, model.ParentBranchID, model.BranchTypeID);
                    return View(model);
                }

                await _branchRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }

            await PopulateViewBags(_sessionHelper.CompanyID, model.ParentBranchID, model.BranchTypeID);
            return View(model);
        }

        private async Task PopulateViewBags(int companyID, int? selectedParentBranchID = null, int? selectedBranchTypeID = null)
        {
            var branches = await _branchRepository.GetByCompanyAsync(companyID);
            var branchTypes = await _branchTypeRepository.GetAllAsync();

            ViewBag.BrchID = new SelectList(branches, "BranchID", "BranchName", selectedParentBranchID);
            ViewBag.BranchTypeID = new SelectList(branchTypes.Select((type, index) => new { Text = type, Value = index + 1 }), "Value", "Text", selectedBranchTypeID);
        }
    }
}
