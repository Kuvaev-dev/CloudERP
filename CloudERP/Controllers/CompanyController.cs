using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Factories;
using CloudERP.Helpers;
using CloudERP.Models;
using DatabaseAccess.Adapters;
using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IFileService _fileService;
        private readonly IFileAdapterFactory _fileAdapterFactory;
        private readonly SessionHelper _sessionHelper;

        public CompanyController(
            ICompanyRepository companyRepository, 
            IFileService fileService,
            IFileAdapterFactory fileAdapterFactory, 
            SessionHelper sessionHelper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(ICompanyRepository));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(IFileService));
            _fileAdapterFactory = fileAdapterFactory ?? throw new ArgumentNullException(nameof(IFileAdapterFactory));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        // GET: Company
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var companies = await _companyRepository.GetAllAsync();
                return View(companies);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Company/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null)
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var company = await _companyRepository.GetByIdAsync(id.Value);
                if (company == null) return HttpNotFound();

                return View(company);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
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

                    var folder = "~/Content/CompanyLogo";
                    var fileName = $"{model.Company.Name}.jpg";

                    if (model.LogoFile != null)
                    {
                        var fileAdapter = _fileAdapterFactory.Create(model.LogoFile);

                        model.Company.Logo = _fileService.UploadPhoto(fileAdapter, folder, fileName);
                    }
                    else
                    {
                        model.Company.Logo = _fileService.SetDefaultPhotoPath("~/Content/CompanyLogo/erp-logo.png");
                    }

                    await _companyRepository.AddAsync(model.Company);

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: Company/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

                var company = await _companyRepository.GetByIdAsync(id.Value);
                if (company == null) return HttpNotFound();

                return View(company);
            }
            catch
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage;
                return RedirectToAction("EP500", "EP");
            }
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
                        var folder = "~/Content/CompanyLogo";
                        var fileName = $"{model.Company.CompanyID}.jpg";

                        var fileAdapter = _fileAdapterFactory.Create(model.LogoFile);
                        var photoPath = _fileService.UploadPhoto(fileAdapter, folder, fileName);

                        model.Company.Logo = photoPath ?? model.Company.Logo;
                    }

                    await _companyRepository.UpdateAsync(model.Company);

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}