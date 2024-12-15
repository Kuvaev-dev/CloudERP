using CloudERP.Helpers;
using CloudERP.Mapping.Base;
using CloudERP.Models;
using Domain.Models;
using Domain.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CloudERP.Controllers
{
    public class SupportController : Controller
    {
        private readonly ISupportTicketService _service;
        private readonly IMapper<SupportTicket, SupportTicketMV> _mapper;
        private readonly SessionHelper _sessionHelper;

        public SupportController(ISupportTicketService service, IMapper<SupportTicket, SupportTicketMV> mapper, SessionHelper sessionHelper)
        {
            _service = service;
            _mapper = mapper;
            _sessionHelper = sessionHelper;
        }

        public ActionResult Support()
        {
            return View(new SupportTicketMV());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SubmitTicket(SupportTicketMV model)
        {
            if (ModelState.IsValid)
            {
                var domainModel = _mapper.MapToDomain(model);
                domainModel.CompanyID = _sessionHelper.CompanyID;
                domainModel.BranchID = _sessionHelper.BranchID;
                domainModel.UserID = _sessionHelper.UserID;
                domainModel.DateCreated = DateTime.Now;
                domainModel.IsResolved = false;

                await _service.CreateAsync(domainModel);
                ViewBag.Message = Resources.Messages.SupportRequestSubmitted;

                return View("Support", new SupportTicketMV());
            }
            return View("Support", model);
        }

        public async Task<ActionResult> AdminList()
        {
            var tickets = await _service.GetAllAsync();
            return View(tickets.Select(_mapper.MapToViewModel));
        }

        [HttpPost]
        public async Task<ActionResult> ResolveTicket(int id)
        {
            await _service.ResolveAsync(id);
            return RedirectToAction("AdminList");
        }
    }
}