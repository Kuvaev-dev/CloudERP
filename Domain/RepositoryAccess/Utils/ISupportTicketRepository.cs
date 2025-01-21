using Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.RepositoryAccess
{
    public interface ISupportTicketRepository
    {
        Task<IEnumerable<SupportTicket>> GetAllAsync();
        Task<SupportTicket> GetByIdAsync(int id);
        Task AddAsync(SupportTicket ticket);
        Task UpdateAsync(SupportTicket ticket);
        Task ResolveAsync(int id);
        Task<int> GetTotalSupportTicketsByCompany(int companyId);
        Task<int> GetTotalResolvedSupportTicketsByCompany(int companyId);
        Task<int> GetTotalPendingSupportTicketsByCompany(int companyId);
    }
}
