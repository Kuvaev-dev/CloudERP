using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using CloudERP.Mapping;
using CloudERP.Models;
using Domain.Services;

namespace CloudERP.Controllers
{
    public class AccountHeadController : Controller
    {
        private readonly IAccountHeadService _service;

        public AccountHeadController(IAccountHeadService service)
        {
            _service = service;
        }

        public async Task<ActionResult> Index()
        {
            var accountHeads = await _service.GetAllAsync();
            return View(accountHeads);
        }

        public ActionResult Create()
        {
            return View(new AccountHeadMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AccountHeadMV model)
        {
            if (ModelState.IsValid)
            {
                model.UserID = Convert.ToInt32(Session["UserID"]);
                await _service.CreateAsync(AccountHeadMapper.MapToDomain(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<ActionResult> Edit(int id)
        {
            var accountHead = await _service.GetByIdAsync(id);
            if (accountHead == null) return HttpNotFound();

            return View(AccountHeadMapper.MapToViewModel(accountHead));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(AccountHeadMV model)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateAsync(AccountHeadMapper.MapToDomain(model));
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}