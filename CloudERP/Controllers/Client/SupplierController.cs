using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Client
{
    public class SupplierController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;
        private readonly IPhoneNumberHelper _phoneNumberHelper;

        public SupplierController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IPhoneNumberHelper phoneNumberHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _phoneNumberHelper = phoneNumberHelper ?? throw new ArgumentNullException(nameof(phoneNumberHelper));
        }

        // GET: All Suppliers
        public async Task<ActionResult> AllSuppliers()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var suppliers = await _httpClient.GetAsync<IEnumerable<Supplier>>("supplierapi/getall");
                return View(suppliers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var suppliers = await _httpClient.GetAsync<List<Supplier>>(
                    $"supplierapi/getbysetting?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(suppliers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SubBranchSupplier()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var suppliers = await _httpClient.GetAsync<IEnumerable<Supplier>>(
                    $"supplierapi/getbybranch?branchId={_sessionHelper.BrchID}");
                return View(suppliers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Supplier/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null) return RedirectToAction("EP404", "EP");

                var supplier = await _httpClient.GetAsync<Supplier>($"supplierapi/getbyid?id={id}");
                if (supplier == null) return RedirectToAction("EP404", "EP");

                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Supplier/Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new Supplier());
        }

        // POST: Supplier/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Supplier model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PostAsync("supplierapi/create", model);
                    if (success) return RedirectToAction("AllSuppliers");

                    ViewBag.Message = Localization.CloudERP.Messages.Messages.AlreadyExists;
                    return View(model);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Supplier/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null) return RedirectToAction("EP404", "EP");

                var supplier = await _httpClient.GetAsync<Supplier>($"supplierapi/getbyid?id={id}");
                if (supplier == null) return RedirectToAction("EP404", "EP");
                supplier.SupplierConatctNo = _phoneNumberHelper.ExtractNationalNumber(supplier.SupplierConatctNo);
                return View(supplier);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Supplier/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Supplier model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PutAsync($"supplierapi/update?id={model.SupplierID}", model);
                    if (success) return RedirectToAction("AllSuppliers");

                    ViewBag.Message = Localization.CloudERP.Messages.Messages.AlreadyExists;
                    return View(model);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}