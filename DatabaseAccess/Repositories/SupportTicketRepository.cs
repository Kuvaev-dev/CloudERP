using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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

        public async Task<IEnumerable<SupportTicket>> GetAllAsync()
        {
            try
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
                    UserID = t.UserID
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving tickets.", ex);
            }
        }

        public async Task<SupportTicket> GetByIdAsync(int id)
        {
            try
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
                    UserID = entity.UserID
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving ticket with ID {id}.", ex);
            }
        }

        public async Task AddAsync(SupportTicket ticket)
        {
            try
            {
                if (ticket == null) throw new ArgumentNullException(nameof(ticket));

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
                    UserID = ticket.UserID
                };

                _dbContext.tblSupportTicket.Add(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new ticket.", ex);
            }
        }

        public async Task UpdateAsync(SupportTicket ticket)
        {
            try
            {
                if (ticket == null) throw new ArgumentNullException(nameof(ticket));

                var entity = await _dbContext.tblSupportTicket.FindAsync(ticket.TicketID);
                if (entity == null) throw new KeyNotFoundException("Support Ticket not found.");

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

                _dbContext.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
            }
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error updating ticket with ID {ticket.TicketID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
