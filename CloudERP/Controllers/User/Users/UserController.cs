using Domain.Models;
using Domain.UtilsAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CloudERP.Controllers.User.Users
{
    public class UserController : Controller
    {
        private readonly IHttpClientHelper _httpClient;
        private readonly ISessionHelper _sessionHelper;

        public UserController(
            ISessionHelper sessionHelper,
            IHttpClientHelper httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(sessionHelper));
        }

        // GET: User
        public async Task<ActionResult> Index()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var users = await _httpClient.GetAsync<List<Domain.Models.User>>($"userapi/getall");
                return View(users);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User
        public async Task<ActionResult> SubBranchUser()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var users = await _httpClient.GetAsync<List<Domain.Models.User>>(
                    $"userapi/getbybranch?companyId={_sessionHelper.CompanyID}&branchTypeId={_sessionHelper.BranchTypeID}&branchId={_sessionHelper.BrchID}");
                return View(users ?? []);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Details/{id}
        public async Task<ActionResult> Details(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var user = await _httpClient.GetAsync<Domain.Models.User>($"userapi/getbyid?id={id}");
                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Create
        public async Task<ActionResult> Create()
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            var userTypes = await _httpClient.GetAsync<IEnumerable<UserType>>($"usertypeapi/getall");
            ViewBag.UserTypeList = userTypes?.Select(ut => new SelectListItem
            {
                Value = ut.UserTypeID.ToString(),
                Text = ut.UserTypeName
            });

            return View(new Domain.Models.User());
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Domain.Models.User model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    var success = await _httpClient.PostAsync($"user/create", model);
                    if (success) return RedirectToAction("EP500", "EP");

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // GET: User/Edit/{id}
        public async Task<ActionResult> Edit(int id)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                var user = await _httpClient.GetAsync<Domain.Models.User>($"userapi/getbyid?id={id}");

                var userTypes = await _httpClient.GetAsync<IEnumerable<UserType>>($"usertypeapi/getall");
                ViewBag.UserTypeList = userTypes?.Select(ut => new SelectListItem
                {
                    Value = ut.UserTypeID.ToString(),
                    Text = ut.UserTypeName
                });

                return View(user);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        // POST: User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Domain.Models.User model)
        {
            if (!_sessionHelper.IsAuthenticated)
                return RedirectToAction("Login", "Home");

            try
            {
                if (ModelState.IsValid)
                {
                    var response = await _httpClient.PutAsync($"userapi/update?id={model.UserID}", model);

                    return RedirectToAction("Index");
                }

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Localization.CloudERP.Messages.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}