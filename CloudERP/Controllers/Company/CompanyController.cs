using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CloudERP.Helpers;
using DatabaseAccess.Factories;
using Domain.Models;
using Domain.RepositoryAccess;
using Utils.Interfaces;

namespace CloudERP.Controllers
{
    public class CompanyController : Controller
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IFileService _fileService;
        private readonly IFileAdapterFactory _fileAdapterFactory;
        private readonly SessionHelper _sessionHelper;

        private const string COMPANY_LOGO_PATH = "~/Content/CompanyLogo";
        private const string DEFAULT_COMPANY_LOGO_PATH = "~/Content/CompanyLogo/erp-logo.png";

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
                if (id == null) return RedirectToAction("EP404", "EP");

                var company = await _companyRepository.GetByIdAsync(id.Value);
                if (company == null) return RedirectToAction("EP404", "EP");

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

            return View(new Company());
        }

        // POST: Company/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Company model, HttpPostedFileBase logo)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    if (await _companyRepository.CheckCompanyExistsAsync(model.Name))
                    {
                        ModelState.AddModelError("Name", Resources.Messages.AlreadyExists);
                        return View(model);
                    }

                    if (logo != null)
                    {
                        var fileName = $"{model.Name}.jpg";

                        var fileAdapter = _fileAdapterFactory.Create(logo);
                        model.Logo = _fileService.UploadPhoto(fileAdapter, COMPANY_LOGO_PATH, fileName);
                    }
                    else
                    {
                        model.Logo = _fileService.SetDefaultPhotoPath(DEFAULT_COMPANY_LOGO_PATH);
                    }

                    await _companyRepository.AddAsync(model);

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
                if (id == null) return RedirectToAction("EP404", "EP");

                var company = await _companyRepository.GetByIdAsync(id.Value);
                if (company == null) return RedirectToAction("EP404", "EP");

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
        public async Task<ActionResult> Edit(Company model, HttpPostedFileBase logo)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    if (logo != null)
                    {
                        var fileName = $"{model.Name}.jpg";

                        var fileAdapter = _fileAdapterFactory.Create(logo);
                        model.Logo = _fileService.UploadPhoto(fileAdapter, COMPANY_LOGO_PATH, fileName);
                    }
                    else
                    {
                        model.Logo = _fileService.SetDefaultPhotoPath(DEFAULT_COMPANY_LOGO_PATH);
                    }

                    await _companyRepository.UpdateAsync(model);

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