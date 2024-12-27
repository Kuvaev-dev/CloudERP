using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly SessionHelper _sessionHelper;

        public SupplierController(ISupplierRepository supplierRepository, SessionHelper sessionHelper)
        {
            _supplierRepository = supplierRepository;
            _sessionHelper = sessionHelper;
        }

        // GET: All Suppliers
        public async Task<ActionResult> AllSuppliers()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var suppliers = await _supplierRepository.GetAllAsync();
            return View(suppliers);
        }

        // GET: Supplier
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            int companyID = _sessionHelper.CompanyID;
            int branchID = _sessionHelper.BranchID;

            var suppliers = await _supplierRepository.GetByCompanyAndBranchAsync(companyID, branchID);
            return View(suppliers);
        }

        // GET: Sub Branch Supplier
        public async Task<ActionResult> SubBranchSupplier()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            int branchID = _sessionHelper.BrchID;
            var branchSuppliers = await _supplierRepository.GetSuppliersByBranchesAsync(branchID);

            return View(branchSuppliers);
        }

        // GET: Supplier/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var supplier = await _supplierRepository.GetByIdAsync(id.Value);
            if (supplier == null)
                return HttpNotFound();

            return View(supplier);
        }

        // GET: Supplier/SupplierDetails/5
        public async Task<ActionResult> SupplierDetails(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var supplier = await _supplierRepository.GetByIdAsync(id.Value);
            if (supplier == null)
                return HttpNotFound();

            return View(supplier);
        }

        // GET: Supplier/Create
        public ActionResult Create()
        {
            return View(new Supplier());
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Supplier model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            model.CompanyID = _sessionHelper.CompanyID;
            model.BranchID = _sessionHelper.BranchID;
            model.UserID = _sessionHelper.UserID;

            if (ModelState.IsValid)
            {
                var existingSupplier = await _supplierRepository.GetByNameAndContactAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID, model.SupplierName, model.SupplierConatctNo);
                if (existingSupplier == null)
                {
                    await _supplierRepository.AddAsync(model);

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

            var supplier = await _supplierRepository.GetByIdAsync(id.Value);
            if (supplier == null)
                return HttpNotFound();

            return View(supplier);
        }

        // POST: Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Supplier model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            int userID = _sessionHelper.UserID;
            model.UserID = userID;

            if (ModelState.IsValid)
            {
                var existingSupplier = await _supplierRepository.GetByNameAndContactAsync(model.CompanyID, model.BranchID, model.SupplierName, model.SupplierConatctNo);
                if (existingSupplier == null || existingSupplier.SupplierID == model.SupplierID)
                {
                    await _supplierRepository.UpdateAsync(model);

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