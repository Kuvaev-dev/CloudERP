using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Helpers;
using CloudERP.Models;
using Domain.RepositoryAccess;

namespace CloudERP.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly SessionHelper _sessionHelper;

        public CompanyController(ICompanyRepository companyRepository, SessionHelper sessionHelper)
        {
            _companyRepository = companyRepository;
            _sessionHelper = sessionHelper;
        }

        // GET: Company
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var companies = await _companyRepository.GetAllAsync();

            return View(companies);
        }

        // GET: Company/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var company = await _companyRepository.GetByIdAsync(id.Value);
            if (company == null) return HttpNotFound();

            return View(company);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            return View(new CompanyMV());
        }

        public async Task<bool> CheckCompanyExistsAsync(string name)
        {
            var companies = await _companyRepository.GetAllAsync();
            return companies.Any(c => c.Name == name);
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
                    if (await CheckCompanyExistsAsync(model.Company.Name))
                    {
                        ModelState.AddModelError("Name", Resources.Messages.AlreadyExists);
                        return View(model);
                    }

                    model.Company.Logo = model.LogoFile != null
                        ? FileHelper.UploadPhoto(model.LogoFile, "~/Content/CompanyLogo", $"{model.Company.Name}.jpg")
                        : "~/Content/CompanyLogo/erp-logo.png";

                    await _companyRepository.AddAsync(model.Company);

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

            var company = await _companyRepository.GetByIdAsync(id.Value);
            if (company == null) return HttpNotFound();

            return View(company);
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
                        var filePath = FileHelper.UploadPhoto(model.LogoFile, "~/Content/CompanyLogo", $"{model.Company.CompanyID}.jpg");
                        model.Company.Logo = filePath ?? model.Company.Logo;
                    }

                    await _companyRepository.UpdateAsync(model.Company);

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