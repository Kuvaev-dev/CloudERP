using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.User.Stuff
{
    public class CustomerController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public CustomerController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(HttpClientHelper));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: AllCustomers
        public async Task<ActionResult> AllCustomers()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var customers = await _httpClient.GetAsync<List<Customer>>("customer");
                return View(customers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при получении клиентов: " + ex.Message;
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
                var customers = await _httpClient.GetAsync<List<Customer>>($"customer/{_sessionHelper.CompanyID}/{_sessionHelper.BranchID}");
                return View(customers);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при получении клиентов: " + ex.Message;
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
                var customer = await _httpClient.GetAsync<Customer>($"customer/{id}");
                return View(customer);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при получении данных о клиенте: " + ex.Message;
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

            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PostAsync<Customer>("customer", model);
                    if (success) return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при создании клиента: " + ex.Message;
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
                var customer = await _httpClient.GetAsync<Customer>($"customer/{id}");
                return View(customer);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при получении данных клиента: " + ex.Message;
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

            try
            {
                model.UserID = _sessionHelper.UserID;

                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PutAsync<Customer>($"customer/update/{model.CustomerID}", model);
                    if (success) return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ошибка при обновлении клиента: " + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}
