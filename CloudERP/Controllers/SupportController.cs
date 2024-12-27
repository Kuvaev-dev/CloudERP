using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SupportController : Controller
    {
        private readonly ISupportTicketRepository _supportTicketRepository;
        private readonly SessionHelper _sessionHelper;

        public SupportController(ISupportTicketRepository supportTicketRepository, SessionHelper sessionHelper)
        {
            _supportTicketRepository = supportTicketRepository;
            _sessionHelper = sessionHelper;
        }

        public ActionResult Support()
        {
            return View(new SupportTicket());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitTicket(SupportTicket model)
        {
            if (ModelState.IsValid)
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;
                model.DateCreated = DateTime.Now;
                model.IsResolved = false;

                await _supportTicketRepository.AddAsync(model);
                ViewBag.Message = Resources.Messages.SupportRequestSubmitted;

                return View("Support", new SupportTicket());
            }
            return View("Support", model);
        }

        public async Task<ActionResult> AdminList()
        {
            var tickets = await _supportTicketRepository.GetAllAsync();
            return View(tickets);
        }

        [HttpPost]
        public async Task<ActionResult> ResolveTicket(int id)
        {
            await _supportTicketRepository.ResolveAsync(id);
            return RedirectToAction("AdminList");
        }
    }
}