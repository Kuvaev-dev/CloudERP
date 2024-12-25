using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly SessionHelper _sessionHelper;

        public CustomerController(ICustomerRepository customerRepository, SessionHelper sessionHelper)
        {
            _customerRepository = customerRepository;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> AllCustomers()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var customers = await _customerRepository.GetAllAsync();
            return View(customers);
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var customers = await _customerRepository.GetByCompanyAndBranchAsync(_sessionHelper.CompanyID, _sessionHelper.BranchID);
            return View(customers);
        }

        public async Task<ActionResult> SubBranchCustomer()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(await _customerRepository.GetByBranchesAsync(_sessionHelper.BranchID));
        }

        public async Task<ActionResult> Details(int? id) => await GetCustomerDetails(id);

        public async Task<ActionResult> CustomerDetails(int? id) => await GetCustomerDetails(id);

        public async Task<ActionResult> GetCustomerDetails(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var customer = await _customerRepository.GetByIdAsync(id.Value);
            if (customer == null) return HttpNotFound();

            return View(customer);
        }

        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new Customer());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                await _customerRepository.AddAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null) return HttpNotFound();

            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Customer model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (ModelState.IsValid)
            {
                await _customerRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }

            return View(model);
        }
    }
}