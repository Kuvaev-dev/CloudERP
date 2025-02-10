using CloudERP.Helpers;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class FinancialYearViewController : Controller
    {
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly SessionHelper _sessionHelper;

        public FinancialYearViewController(
            IFinancialYearRepository financialYearRepository, 
            SessionHelper sessionHelper)
        {
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: FinancialYear
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var financialYears = await _financialYearRepository.GetAllAsync();
                if (financialYears == null) return RedirectToAction("EP404", "EP");

                return View(financialYears);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}