using System.Net;
using System;
using System.Web.Mvc;
using CloudERP.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class UserTypeController : Controller
    {
        private readonly IUserTypeService _service;

        public UserTypeController(IUserTypeService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            var userTypes = _service.GetAll();
            return View(userTypes);
        }

        // GET: UserType/Details/5
        public ActionResult Details(int id)
        {
            try
            {
                if (string.IsNullOrEmpty(Convert.ToString(Session["CompanyID"])))
                {
                    return RedirectToAction("Login", "Home");
                }

                var userType = _service.GetById(id);
                return View(userType);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public ActionResult Create()
        {
            return View(new UserTypeMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserTypeMV model)
        {
            if (ModelState.IsValid)
            {
                var userType = new Domain.Models.UserType
                {
                    UserTypeName = model.UserTypeName
                };

                _service.Create(userType);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var userType = _service.GetById(id);
            if (userType == null) return HttpNotFound();

            var model = new UserTypeMV
            {
                UserTypeID = userType.UserTypeID,
                UserTypeName = userType.UserTypeName
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserTypeMV model)
        {
            if (ModelState.IsValid)
            {
                var userType = new Domain.Models.UserType
                {
                    UserTypeID = model.UserTypeID,
                    UserTypeName = model.UserTypeName
                };

                _service.Update(userType);
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}