using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Services;
using CloudERP.Mapping.Base;
using Domain.Models;

namespace CloudERP.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper<Supplier, SupplierMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public SupplierController(ISupplierService supplierService, IMapper<Supplier, SupplierMV> mapper, SessionHelper sessionHelper)
        {
            _supplierService = supplierService;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        // GET: All Suppliers
        public async Task<ActionResult> AllSuppliers()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var suppliers = await _supplierService.GetAllAsync();
            return View(suppliers);
        }

        // GET: Supplier
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            int companyID = _sessionHelper.CompanyID;
            int branchID = _sessionHelper.BranchID;

            var suppliers = await _supplierService.GetByCompanyAndBranchAsync(companyID, branchID);
            return View(suppliers);
        }

        // GET: Sub Branch Supplier
        public async Task<ActionResult> SubBranchSupplier()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            int branchID = _sessionHelper.BrchID;
            var branchSuppliers = await _supplierService.GetSuppliersByBranchesAsync(branchID);

            return View(branchSuppliers);
        }

        // GET: Supplier/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var supplier = await _supplierService.GetByIdAsync(id.Value);
            if (supplier == null)
                return HttpNotFound();

            return View(supplier);
        }

        // GET: Supplier/SupplierDetails/5
        public async Task<ActionResult> SupplierDetails(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var supplier = await _supplierService.GetByIdAsync(id.Value);
            if (supplier == null)
                return HttpNotFound();

            return View(supplier);
        }

        // GET: Supplier/Create
        public ActionResult Create()
        {
            return View(new SupplierMV());
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SupplierMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;
            model.BranchID = _sessionHelper.BranchID;
            model.UserID = _sessionHelper.UserID;

            if (ModelState.IsValid)
            {
                var existingSupplier = await _supplierService.GetByNameAndContactAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, model.SupplierName, model.SupplierConatctNo);
                if (existingSupplier == null)
                {
                    var supplier = _mapper.MapToDomain(model);
                    await _supplierService.CreateAsync(supplier);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = Resources.Messages.AlreadyExists;
                }
            }

            return View(model);
        }

        // GET: Supplier/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var supplier = await _supplierService.GetByIdAsync(id.Value);
            if (supplier == null)
                return HttpNotFound();

            return View(_mapper.MapToViewModel(supplier));
        }

        // POST: Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SupplierMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            int userID = _sessionHelper.UserID;
            model.UserID = userID;

            if (ModelState.IsValid)
            {
                var existingSupplier = await _supplierService.GetByNameAndContactAsync(model.CompanyID, model.BranchID, model.SupplierName, model.SupplierConatctNo);
                if (existingSupplier == null || existingSupplier.SupplierID == model.SupplierID)
                {
                    var supplier = _mapper.MapToDomain(model);
                    await _supplierService.UpdateAsync(supplier);

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Message = Resources.Messages.AlreadyExists;
                }
            }

            return View(model);
        }
    }
}