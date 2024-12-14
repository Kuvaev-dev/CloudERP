using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Mapping;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _service;
        private readonly IMapper<Customer, CustomerMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public CustomerController(ICustomerService service, IMapper<Customer, CustomerMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> AllCustomers()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var customers = await _service.GetAllCustomersAsync();
            return View(customers);
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var customers = await _service.GetCustomersByCompanyAndBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(customers);
        }

        public async Task<ActionResult> SubBranchCustomer()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            int branchId = _sessionHelper.BranchID;

            var customers = await _service.GetSubBranchCustomersAsync(branchId);

            var model = CustomerMapper.MapToBranchsCustomersMV(customers);

            return View(model);
        }

        public async Task<ActionResult> Details(int? id) => await GetCustomerDetails(id);

        public async Task<ActionResult> CustomerDetails(int? id) => await GetCustomerDetails(id);

        public async Task<ActionResult> GetCustomerDetails(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var customer = await _service.GetCustomerByIdAsync(id.Value);
            if (customer == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(customer));
        }

        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new CustomerMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CustomerMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                await _service.CreateCustomerAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var customer = await _service.GetCustomerByIdAsync(id);
            if (customer == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(customer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CustomerMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                await _service.UpdateCustomerAsync(_mapper.MapToDomain(model));
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}