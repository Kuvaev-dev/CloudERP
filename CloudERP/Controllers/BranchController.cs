using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class BranchController : Controller
    {
        private readonly IBranchService _service;
        private readonly IMapper<Branch, BranchMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public BranchController(IBranchService service, IMapper<Branch, BranchMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        // GET: Branch
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var branches = await _service.GetBranchesByFilterAsync(_sessionHelper.CompanyID, _sessionHelper.BranchTypeID, _sessionHelper.BranchID);

            return View(branches.Select(_mapper.MapToViewModel));
        }

        // GET: Branch/Create
        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            await PopulateViewBags(_sessionHelper.CompanyID);

            return View(new BranchMV());
        }

        // POST: Branch/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BranchMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;

            if (ModelState.IsValid)
            {
                if (await _service.CheckBranchExistsAsync(_mapper.MapToDomain(model)))
                {
                    ModelState.AddModelError("", Resources.Messages.BranchEists);
                    await PopulateViewBags(_sessionHelper.CompanyID);
                    return View(model);
                }

                await _service.CreateAsync(_mapper.MapToDomain(model));
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

            var branch = await _service.GetByIdAsync(id);
            if (branch == null) return HttpNotFound();

            await PopulateViewBags(_sessionHelper.CompanyID, branch.ParentBranchID, branch.BranchTypeID);

            var viewModel = _mapper.MapToViewModel(branch);
            return View(viewModel);
        }

        // POST: Branch/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BranchMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;

            if (ModelState.IsValid)
            {
                if (await _service.CheckBranchExistsAsync(_mapper.MapToDomain(model), isEdit: true))
                {
                    ModelState.AddModelError("", Resources.Messages.BranchEists);
                    await PopulateViewBags(_sessionHelper.CompanyID, model.ParentBranchID, model.BranchTypeID);
                    return View(model);
                }

                await _service.UpdateAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }

            await PopulateViewBags(_sessionHelper.CompanyID, model.ParentBranchID, model.BranchTypeID);
            return View(model);
        }

        private async Task PopulateViewBags(int companyID, int? selectedParentBranchID = null, int? selectedBranchTypeID = null)
        {
            var branches = await _service.GetBranchesByCompanyAsync(companyID);
            var branchTypes = await _service.GetBranchTypesAsync();

            ViewBag.BrchID = new SelectList(branches, "BranchID", "BranchName", selectedParentBranchID);
            ViewBag.BranchTypeID = new SelectList(branchTypes.Select((type, index) => new { Text = type, Value = index + 1 }), "Value", "Text", selectedBranchTypeID);
        }
    }
}
