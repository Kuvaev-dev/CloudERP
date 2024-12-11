using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public interface IAccountHeadRepository
    {
        Task<IEnumerable<tblAccountHead>> GetAllAsync();
        Task<tblAccountHead> GetByIdAsync(int id);
        Task AddAsync(tblAccountHead accountHead);
        Task UpdateAsync(tblAccountHead accountHead);
        Task DeleteAsync(tblAccountHead accountHead);
    }

    public class AccountHeadRepository : IAccountHeadRepository
    {
        private readonly CloudDBEntities _dbContext;

        public AccountHeadRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<tblAccountHead>> GetAllAsync()
        {
            return await _dbContext.tblAccountHead.Include(e => e.tblUser).ToListAsync();
        }

        public async Task<tblAccountHead> GetByIdAsync(int id)
        {
            return await _dbContext.tblAccountHead.FirstOrDefaultAsync(ah => ah.AccountHeadID == id);
        }

        public async Task AddAsync(tblAccountHead accountHead)
        {
            _dbContext.tblAccountHead.Add(accountHead);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(tblAccountHead accountHead)
        {
            var entity = await _dbContext.tblAccountHead.FindAsync(accountHead.AccountHeadID);
            if (entity == null) throw new KeyNotFoundException("AccountHead not found.");

            entity.AccountHeadName = accountHead.AccountHeadName;
            entity.Code = accountHead.Code;
            entity.UserID = accountHead.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(tblAccountHead accountHead)
        {
            _dbContext.tblAccountHead.Remove(accountHead);
            await _dbContext.SaveChangesAsync();
        }
    }
}
