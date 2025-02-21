using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Utilities
{
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly CloudDBEntities _dbContext;

        public SupportTicketRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<SupportTicket>> GetAllAsync()
        {
            var entities = await _dbContext.tblSupportTicket.ToListAsync();

            return entities.Select(t => new SupportTicket
            {
                TicketID = t.TicketID,
                Subject = t.Subject,
                Name = t.Name,
                Email = t.Email,
                Message = t.Message,
                DateCreated = t.DateCreated,
                IsResolved = t.IsResolved,
                CompanyID = t.CompanyID,
                BranchID = t.BranchID,
                UserID = t.UserID,
                AdminResponse = t.AdminResponse,
                RespondedBy = t.RespondedBy,
                ResponseDate = t.ResponseDate
            });
        }

        public async Task<SupportTicket?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblSupportTicket.FindAsync(id);

            return entity == null ? null : new SupportTicket
            {
                TicketID = entity.TicketID,
                Subject = entity.Subject,
                Name = entity.Name,
                Email = entity.Email,
                Message = entity.Message,
                DateCreated = entity.DateCreated,
                IsResolved = entity.IsResolved,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                UserID = entity.UserID,
                AdminResponse = entity.AdminResponse,
                RespondedBy = entity.RespondedBy,
                ResponseDate = entity.ResponseDate
            };
        }

        public async Task<IEnumerable<SupportTicket>> GetByUserIdAsync(int userId)
        {
            var entities = await _dbContext.tblSupportTicket
                .Where(t => t.UserID == userId)
                .OrderByDescending(t => t.DateCreated)
                .ToListAsync();

            return entities.Select(t => new SupportTicket
            {
                TicketID = t.TicketID,
                Subject = t.Subject,
                Name = t.Name,
                Email = t.Email,
                Message = t.Message,
                DateCreated = t.DateCreated,
                IsResolved = t.IsResolved,
                CompanyID = t.CompanyID,
                BranchID = t.BranchID,
                UserID = t.UserID,
                AdminResponse = t.AdminResponse,
                RespondedBy = t.RespondedBy,
                ResponseDate = t.ResponseDate
            });
        }

        public async Task AddAsync(SupportTicket ticket)
        {
            var entity = new tblSupportTicket
            {
                TicketID = ticket.TicketID,
                Subject = ticket.Subject,
                Name = ticket.Name,
                Email = ticket.Email,
                Message = ticket.Message,
                DateCreated = ticket.DateCreated,
                IsResolved = ticket.IsResolved,
                CompanyID = ticket.CompanyID,
                BranchID = ticket.BranchID,
                UserID = ticket.UserID,
                AdminResponse = ticket.AdminResponse,
                RespondedBy = ticket.RespondedBy,
                ResponseDate = ticket.ResponseDate
            };

            _dbContext.tblSupportTicket.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(SupportTicket ticket)
        {
            var entity = await _dbContext.tblSupportTicket.FindAsync(ticket.TicketID);

            entity.TicketID = ticket.TicketID;
            entity.Subject = ticket.Subject;
            entity.Name = ticket.Name;
            entity.Email = ticket.Email;
            entity.Message = ticket.Message;
            entity.DateCreated = ticket.DateCreated;
            entity.IsResolved = ticket.IsResolved;
            entity.CompanyID = ticket.CompanyID;
            entity.BranchID = ticket.BranchID;
            entity.UserID = ticket.UserID;
            entity.AdminResponse = ticket.AdminResponse;
            entity.RespondedBy = ticket.RespondedBy;
            entity.ResponseDate = ticket.ResponseDate;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task ResolveAsync(int id)
        {
            var ticket = await _dbContext.tblSupportTicket.FindAsync(id);
            if (ticket != null)
            {
                ticket.IsResolved = true;
                await _dbContext.SaveChangesAsync();
            }
        }

        public Task<int> GetTotalSupportTicketsByCompany(int companyId)
        {
            return _dbContext.tblSupportTicket.CountAsync(t => t.CompanyID == companyId);
        }

        public Task<int> GetTotalResolvedSupportTicketsByCompany(int companyId)
        {
            return _dbContext.tblSupportTicket.CountAsync(t => t.CompanyID == companyId && t.IsResolved);
        }

        public Task<int> GetTotalPendingSupportTicketsByCompany(int companyId)
        {
            return _dbContext.tblSupportTicket.CountAsync(t => t.CompanyID == companyId && !t.IsResolved);
        }
    }
}
