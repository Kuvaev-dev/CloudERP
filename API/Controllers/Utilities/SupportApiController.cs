using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Utilities
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize]
    public class SupportApiController : ControllerBase
    {
        private readonly ISupportTicketRepository _supportTicketRepository;

        public SupportApiController(ISupportTicketRepository supportTicketRepository)
        {
            _supportTicketRepository = supportTicketRepository ?? throw new ArgumentNullException(nameof(supportTicketRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportTicket>>> GetUserTickets(int userId)
        {
            var tickets = await _supportTicketRepository.GetByUserIdAsync(userId);
            if (tickets == null) return NotFound();
            return Ok(tickets);
        }

        [HttpPost]
        public async Task<ActionResult> SubmitTicket([FromBody] SupportTicket model)
        {
            await _supportTicketRepository.AddAsync(model);
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportTicket>>> GetAdminList()
        {
            var tickets = await _supportTicketRepository.GetAllAsync();
            if (tickets == null) return NotFound();
            return Ok(tickets);
        }

        [HttpGet]
        public async Task<ActionResult<SupportTicket>> GetById(int id)
        {
            var ticket = await _supportTicketRepository.GetByIdAsync(id);
            if (ticket == null) return NotFound();
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<ActionResult> ResolveTicket(int id, [FromBody] SupportTicket supportTicket)
        {
            var ticket = await _supportTicketRepository.GetByIdAsync(id);
            if (ticket == null) return NotFound();

            ticket.AdminResponse = supportTicket.AdminResponse;
            ticket.RespondedBy = supportTicket.RespondedBy;
            ticket.ResponseDate = supportTicket.ResponseDate;
            ticket.IsResolved = supportTicket.IsResolved;

            await _supportTicketRepository.UpdateAsync(ticket);
            return Ok();
        }
    }
}