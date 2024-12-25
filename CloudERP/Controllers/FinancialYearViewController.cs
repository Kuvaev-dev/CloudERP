using Domain.RepositoryAccess;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class FinancialYearViewController : Controller
    {
        private readonly IFinancialYearRepository _financialYearRepository;

        public FinancialYearViewController(IFinancialYearRepository financialYearRepository)
        {
            _financialYearRepository = financialYearRepository;
        }

        // GET: FinancialYear
        public async Task<ActionResult> Index()
        {
            var financialYears = await _financialYearRepository.GetAllAsync();
            return View(financialYears);
        }
    }
}