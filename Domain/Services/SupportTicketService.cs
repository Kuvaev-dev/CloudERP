using DatabaseAccess.Repositories;
using DatabaseAccess;
using Domain.Mapping.Base;
using Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface ISupportTicketService
    {
        Task<IEnumerable<SupportTicket>> GetAllAsync();
        Task<SupportTicket> GetByIdAsync(int id);
        Task CreateAsync(SupportTicket ticket);
        Task ResolveAsync(int id);
    }

    public class SupportTicketService : ISupportTicketService
    {
        private readonly ISupportTicketRepository _repository;
        private readonly IMapper<SupportTicket, tblSupportTicket> _mapper;

        public SupportTicketService(ISupportTicketRepository repository, IMapper<SupportTicket, tblSupportTicket> mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupportTicket>> GetAllAsync()
        {
            var tickets = await _repository.GetAllAsync();
            return tickets.Select(_mapper.MapToDomain);
        }

        public async Task<SupportTicket> GetByIdAsync(int id)
        {
            var ticket = await _repository.GetByIdAsync(id);
            return ticket == null ? null : _mapper.MapToDomain(ticket);
        }

        public async Task CreateAsync(SupportTicket ticket)
        {
            var dbModel = _mapper.MapToDatabase(ticket);
            await _repository.AddAsync(dbModel);
        }

        public async Task ResolveAsync(int id)
        {
            var ticket = await _repository.GetByIdAsync(id);
            if (ticket != null)
            {
                ticket.IsResolved = true;
                await _repository.UpdateAsync(ticket);
            }
        }
    }
}
