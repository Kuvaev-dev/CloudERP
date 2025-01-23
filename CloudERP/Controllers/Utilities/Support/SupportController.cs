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

        public SupportController(
            ISupportTicketRepository supportTicketRepository, 
            SessionHelper sessionHelper)
        {
            _supportTicketRepository = supportTicketRepository ?? throw new ArgumentNullException(nameof(ISupportTicketRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
        }

        public ActionResult Support()
        {
            return View(new SupportTicket());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitTicket(SupportTicket model)
        {
            try
            {
                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;
                model.DateCreated = DateTime.Now;
                model.IsResolved = false;

                if (ModelState.IsValid)
                {
                    await _supportTicketRepository.AddAsync(model);
                    ViewBag.Message = Resources.Messages.SupportRequestSubmitted;

                    return View("Support", new SupportTicket());
                }
                return View("Support", model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        public async Task<ActionResult> AdminList()
        {
            try
            {
                var tickets = await _supportTicketRepository.GetAllAsync();
                return View(tickets);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResolveTicket(int id)
        {
            try
            {
                await _supportTicketRepository.ResolveAsync(id);
                return RedirectToAction("AdminList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = Resources.Messages.UnexpectedErrorMessage + ex.Message;
                return RedirectToAction("EP500", "EP");
            }
        }
    }
}