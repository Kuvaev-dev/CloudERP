using Domain.Services;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class FinancialYearViewController : Controller
    {
        private readonly IFinancialYearService _service;

        public FinancialYearViewController(IFinancialYearService service)
        {
            _service = service;
        }

        // GET: FinancialYear
        public async Task<ActionResult> Index()
        {
            var financialYears = await _service.GetAllAsync();
            return View(financialYears);
        }
    }
}