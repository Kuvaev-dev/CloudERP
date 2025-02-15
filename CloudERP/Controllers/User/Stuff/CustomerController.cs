using CloudERP.Helpers;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class CustomerController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly SessionHelper _sessionHelper;
        private readonly string _apiBaseUrl;

        public CustomerController()
        {
            _httpClient = new HttpClient();
            _apiBaseUrl = "https://yourapiurl.com/api/customers"; // Замените на URL вашего API
        }

        // GET: AllCustomers
        public async Task<ActionResult> AllCustomers()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var response = await _httpClient.GetAsync(_apiBaseUrl);
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении клиентов.";
                    return RedirectToAction("EP500", "EP");
                }

                var customers = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
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
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}?companyId={_sessionHelper.CompanyID}&branchId={_sessionHelper.BranchID}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении клиентов.";
                    return RedirectToAction("EP500", "EP");
                }

                var customers = await response.Content.ReadAsAsync<IEnumerable<Customer>>();
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
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении данных о клиенте.";
                    return RedirectToAction("EP500", "EP");
                }

                var customer = await response.Content.ReadAsAsync<Customer>();
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
                    var response = await _httpClient.PostAsJsonAsync(_apiBaseUrl, model);
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Ошибка при создании клиента.";
                        return RedirectToAction("EP500", "EP");
                    }

                    return RedirectToAction("Index");
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
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] = "Ошибка при получении данных клиента.";
                    return RedirectToAction("EP500", "EP");
                }

                var customer = await response.Content.ReadAsAsync<Customer>();
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
                    var response = await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{model.CustomerID}", model);
                    if (!response.IsSuccessStatusCode)
                    {
                        TempData["ErrorMessage"] = "Ошибка при обновлении клиента.";
                        return RedirectToAction("EP500", "EP");
                    }

                    return RedirectToAction("Index");
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
