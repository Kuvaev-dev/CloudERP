using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Localization.CloudERP.Messages;

namespace CloudERP.Controllers.Client
{
    public class CustomerController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;
        private readonly IPhoneNumberHelper _phoneNumberHelper;

        public CustomerController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient,
            IPhoneNumberHelper phoneNumberHelper)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
            _phoneNumberHelper = phoneNumberHelper ?? throw new ArgumentNullException(nameof(phoneNumberHelper));
        }

        // GET: AllCustomers
        public async Task<ActionResult> AllCustomers()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var customers = await _httpClient.GetAsync<IEnumerable<Customer>>("customerapi/getall");
                return View(customers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Index
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var customers = await _httpClient.GetAsync<IEnumerable<Customer>>(
                    $"customerapi/getbysetting?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                return View(customers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> SubBranchCustomer()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (HttpContext.Session.GetInt32("BrchID") == null) return View();
                var customers = await _httpClient.GetAsync<IEnumerable<Customer>>(
                    $"customerapi/getbysetting?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BrchID}");

                return View(customers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var customer = await _httpClient.GetAsync<Customer>($"customerapi/getbyid?id={id}");
                return View(customer);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new Customer());
        }

        // POST: Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;
                
                var success = await _httpClient.PostAsync("customerapi/create", model);
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

        // GET: Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var customer = await _httpClient.GetAsync<Customer>($"customerapi/getbyid?id={id}");
                customer.CustomerContact = _phoneNumberHelper.ExtractNationalNumber(customer.CustomerContact);
                return View(customer);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Customer model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (!ModelState.IsValid) return View(model);

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                var success = await _httpClient.PutAsync($"customer/update?id={model.CustomerID}", model);
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
    }
}
