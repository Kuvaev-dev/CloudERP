using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class FinancialYearController : Controller
    {
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly SessionHelper _sessionHelper;

        public FinancialYearController(IFinancialYearRepository financialYearRepository, SessionHelper sessionHelper)
        {
            _financialYearRepository = financialYearRepository;
            _sessionHelper = sessionHelper;
        }

        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var financialYears = await _financialYearRepository.GetAllAsync();
                return View(financialYears);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult Create()
        {
            return View(new FinancialYear());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FinancialYear model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _financialYearRepository.AddAsync(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var financialYear = await _financialYearRepository.GetByIdAsync(id);
                if (financialYear == null) return HttpNotFound();

                return View(financialYear);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FinancialYear model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    await _financialYearRepository.UpdateAsync(model);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}