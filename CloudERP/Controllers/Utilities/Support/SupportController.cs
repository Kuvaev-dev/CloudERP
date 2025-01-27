using CloudERP.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SupportController : Controller
    {
        private readonly ISupportTicketRepository _supportTicketRepository;
        private readonly IUserRepository _userRepository;
        private readonly SessionHelper _sessionHelper;

        public SupportController(
            ISupportTicketRepository supportTicketRepository, 
            SessionHelper sessionHelper,
            IUserRepository userRepository)
        {
            _supportTicketRepository = supportTicketRepository ?? throw new ArgumentNullException(nameof(ISupportTicketRepository));
            _sessionHelper = sessionHelper ?? throw new ArgumentNullException(nameof(SessionHelper));
            _userRepository = userRepository;
        }

        public async Task<ActionResult> Support()
        {
            try
            {
                var userTickets = await _supportTicketRepository.GetByUserIdAsync(_sessionHelper.UserID);
                ViewBag.UserTickets = userTickets;
                return View(new SupportTicket());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла ошибка: " + ex.Message;
                return RedirectToAction("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitTicket(SupportTicket model)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(_sessionHelper.UserID);
                var userTickets = await _supportTicketRepository.GetByUserIdAsync(_sessionHelper.UserID);

                model.CompanyID = _sessionHelper.CompanyID;
                model.BranchID = _sessionHelper.BranchID;
                model.UserID = _sessionHelper.UserID;
                model.DateCreated = DateTime.Now;
                model.IsResolved = false;
                model.Email = user.Email;
                model.Name = $"{model.DateCreated} - {user.FullName}";
                model.AdminResponse = string.Empty;
                model.RespondedBy = string.Empty;
                model.ResponseDate = null;

                if (ModelState.IsValid)
                {
                    await _supportTicketRepository.AddAsync(model);
                    ViewBag.Message = Resources.Messages.SupportRequestSubmitted;

                    ViewBag.UserTickets = userTickets;

                    return View("Support", new SupportTicket());
                }

                ViewBag.UserTickets = userTickets;
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
        public async Task<ActionResult> ResolveTicket(int id, string responseMessage)
        {
            try
            {
                var ticket = await _supportTicketRepository.GetByIdAsync(id);
                var admin = await _userRepository.GetByIdAsync(_sessionHelper.UserID);

                if (ticket == null)
                {
                    return RedirectToAction("AdminList");
                }

                ticket.AdminResponse = responseMessage;
                ticket.RespondedBy = admin.FullName;
                ticket.ResponseDate = DateTime.Now;
                ticket.IsResolved = true;

                await _supportTicketRepository.UpdateAsync(ticket);

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