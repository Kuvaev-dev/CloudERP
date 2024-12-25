using System.Threading.Tasks;
using System.Web.Mvc;
using Domain.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class FinancialYearController : Controller
    {
        private readonly IFinancialYearRepository _financialYearRepository;

        public FinancialYearController(IFinancialYearRepository financialYearRepository)
        {
            _financialYearRepository = financialYearRepository;
        }

        public async Task<ActionResult> Index()
        {
            var financialYears = await _financialYearRepository.GetAllAsync();
            return View(financialYears);
        }

        public ActionResult Create()
        {
            return View(new FinancialYear());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FinancialYear model)
        {
            if (ModelState.IsValid)
            {
                await _financialYearRepository.AddAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var financialYear = await _financialYearRepository.GetByIdAsync(id);
            if (financialYear == null) return HttpNotFound();

            return View(financialYear);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FinancialYear model)
        {
            if (ModelState.IsValid)
            {
                await _financialYearRepository.UpdateAsync(model);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}