using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupportTicketRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblSupportTicket>> GetAllAsync()
        {
            return await _dbContext.tblSupportTicket.ToListAsync();
        }

        public async Task<tblSupportTicket> GetByIdAsync(int id)
        {
            return await _dbContext.tblSupportTicket.FindAsync(id);
        }

        public async Task AddAsync(tblSupportTicket ticket)
        {
            _dbContext.tblSupportTicket.Add(ticket);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblSupportTicket ticket)
        {
            var existingTicket = await _dbContext.tblSupportTicket.FindAsync(ticket.TicketID);
            if (existingTicket != null)
            {
                _dbContext.Entry(existingTicket).CurrentValues.SetValues(ticket);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
