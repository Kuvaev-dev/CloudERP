using CloudERP.Helpers;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudERP.Controllers.Branch
{
    public class BranchTypeController : Controller
    {
        private readonly HttpClientHelper _httpClient;
        private readonly SessionHelper _sessionHelper;

        public BranchTypeController(
            SessionHelper sessionHelper,
            HttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: BranchType
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var branchTypes = await _httpClient.GetAsync<IEnumerable<BranchType>>("branchtypeapi/getall");
                if (branchTypes == null) return RedirectToAction("EP404", "EP");

                return View(branchTypes);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: BranchType/Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new BranchType());
        }

        // POST: BranchType/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BranchType model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _httpClient.PostAsync("branchtypeapi/create", model);
                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: BranchType/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var accountHead = await _httpClient.GetAsync<BranchType>($"branchtypeapi/getbyid?id={id}");
                if (accountHead == null) return RedirectToAction("EP404", "EP");

                return View(accountHead);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: BranchType/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(BranchType model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _httpClient.PutAsync($"branchtypeapi/update?id={model.BranchTypeID}", model);
                    return RedirectToAction("Index");
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
