using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Mapping;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyService _service;
        private readonly IMapper<Company, CompanyMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public CompanyController(ICompanyService service, IMapper<Company, CompanyMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        // GET: Company
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var companies = await _service.GetAllAsync();
            var viewModel = companies.Select(_mapper.MapToViewModel);

            return View(viewModel);
        }

        // GET: Company/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var company = await _service.GetByIdAsync(id.Value);
            if (company == null) return HttpNotFound();

            var viewModel = _mapper.MapToViewModel(company);
            return View(viewModel);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new CompanyMV());
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CompanyMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    if (await _service.CheckCompanyExistsAsync(model.Name))
                    {
                        ModelState.AddModelError("Name", Resources.Messages.AlreadyExists);
                        return View(model);
                    }

                    model.Logo = model.LogoFile != null
                        ? FileHelper.UploadPhoto(model.LogoFile, "~/Content/CompanyLogo", $"{model.Name}.jpg")
                        : "~/Content/CompanyLogo/erp-logo.png";

                    var domainModel = _mapper.MapToDomain(model);
                    await _service.CreateAsync(domainModel);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage;
                return RedirectToAction("EP500", "EP");
            }

            return View(model);
        }

        // GET: Company/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var company = await _service.GetByIdAsync(id.Value);
            if (company == null) return HttpNotFound();

            var viewModel = _mapper.MapToViewModel(company);
            return View(viewModel);
        }

        // POST: Company/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CompanyMV model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    if (model.LogoFile != null)
                    {
                        var filePath = FileHelper.UploadPhoto(model.LogoFile, "~/Content/CompanyLogo", $"{model.CompanyID}.jpg");
                        model.Logo = filePath ?? model.Logo;
                    }

                    var domainModel = _mapper.MapToDomain(model);
                    await _service.UpdateAsync(domainModel);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage;
                return RedirectToAction("EP500", "EP");
            }

            return View(model);
        }
    }
}