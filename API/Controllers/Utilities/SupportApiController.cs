using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Domain.Models;
using Domain.RepositoryAccess;

namespace API.Controllers
{
    [RoutePrefix("api/support")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SupportApiController : ApiController
    {
        private readonly ISupportTicketRepository _supportTicketRepository;

        public SupportApiController(ISupportTicketRepository supportTicketRepository)
        {
            _supportTicketRepository = supportTicketRepository ?? throw new ArgumentNullException(nameof(supportTicketRepository));
        }

        [HttpGet, Route("user/{userId:int}")]
        public async Task<IHttpActionResult> GetUserTickets(int userId)
        {
            var tickets = await _supportTicketRepository.GetByUserIdAsync(userId);
            return Ok(tickets);
        }

        [HttpPost, Route("submit")]
        public async Task<IHttpActionResult> SubmitTicket([FromBody] SupportTicket model)
        {
            await _supportTicketRepository.AddAsync(model);
            return Ok(new { message = "Заявка успешно отправлена" });
        }

        [HttpGet, Route("admin/list")]
        public async Task<IHttpActionResult> GetAdminList()
        {
            var tickets = await _supportTicketRepository.GetAllAsync();
            return Ok(tickets);
        }

        [HttpPost, Route("resolve/{id}")]
        public async Task<IHttpActionResult> ResolveTicket(int id, [FromBody] string responseMessage)
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