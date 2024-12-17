using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class FinancialYearController : Controller
    {
        private readonly IFinancialYearService _service;
        private readonly IMapper<FinancialYear, FinancialYearMV> _mapper;

        public FinancialYearController(IFinancialYearService service, IMapper<FinancialYear, FinancialYearMV> mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        public async Task<ActionResult> Index()
        {
            var financialYears = await _service.GetAllAsync();
            return View(financialYears);
        }

        public ActionResult Create()
        {
            return View(new FinancialYearMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FinancialYearMV model)
        {
            if (ModelState.IsValid)
            {
                var domainModel = _mapper.MapToDomain(model);
                await _service.CreateAsync(domainModel);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var financialYear = await _service.GetByIdAsync(id);
            if (financialYear == null) return HttpNotFound();

            return View(_mapper.MapToViewModel(financialYear));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FinancialYearMV model)
        {
            if (ModelState.IsValid)
            {
                var domainModel = _mapper.MapToDomain(model);
                await _service.UpdateAsync(domainModel);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}