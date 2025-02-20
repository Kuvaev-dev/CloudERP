using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Utilities
{
    [ApiController]
    [Route("api/[controller]/[action]")]
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
        public async Task<ActionResult<string>> SubmitTicket([FromBody] SupportTicket model)
        {
            await _supportTicketRepository.AddAsync(model);
            return Ok(new { message = "Заявка успешно отправлена" });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupportTicket>>> GetAdminList()
        {
            var tickets = await _supportTicketRepository.GetAllAsync();
            if (tickets == null) return NotFound();
            return Ok(tickets);
        }

        [HttpPost]
        public async Task<ActionResult<string>> ResolveTicket(int id, [FromBody] string responseMessage)
        {
            var ticket = await _supportTicketRepository.GetByIdAsync(id);
            if (ticket == null) return NotFound();

            ticket.AdminResponse = responseMessage;
            ticket.RespondedBy = "Admin";
            ticket.ResponseDate = DateTime.Now;
            ticket.IsResolved = true;

            await _supportTicketRepository.UpdateAsync(ticket);
            return Ok(new { message = "Тикет успешно обработан" });
        }
    }
}