using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Domain.UtilsAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Customers
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CloudDBEntities _dbContext;
        private readonly IBranchHelper _branchHelper;

        public CustomerRepository(
            CloudDBEntities dbContext,
            IBranchHelper branchHelper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _branchHelper = branchHelper ?? throw new ArgumentNullException(nameof(branchHelper));
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            var entities = await _dbContext.tblCustomer
                .Include(c => c.Branch)
                .Include(c => c.Company)
                .Include(c => c.User)
                .ToListAsync();

            return entities.Select(c => new Customer
            {
                CustomerID = c.CustomerID,
                Customername = c.Customername,
                CustomerContact = c.CustomerContact,
                CustomerAddress = c.CustomerAddress,
                Description = c.Description,
                BranchID = c.BranchID,
                CompanyID = c.CompanyID,
                UserID = c.UserID,
                BranchName = c.Branch.BranchName,
                CompanyName = c.Company.Name,
                UserName = c.User.UserName
            });
        }

        public async Task<IEnumerable<Customer>> GetByCompanyAndBranchAsync(int companyId, int branchId)
        {
            var entities = await _dbContext.tblCustomer
                .Where(c => c.CompanyID == companyId && c.BranchID == branchId)
                .Include(c => c.Branch)
                .Include(c => c.Company)
                .Include(c => c.User)
                .ToListAsync();

            return entities.Select(c => new Customer
            {
                CustomerID = c.CustomerID,
                Customername = c.Customername,
                CustomerContact = c.CustomerContact,
                CustomerAddress = c.CustomerAddress,
                Description = c.Description,
                BranchID = c.BranchID,
                CompanyID = c.CompanyID,
                UserID = c.UserID,
                BranchName = c.Branch.BranchName,
                CompanyName = c.Company.Name,
                UserName = c.User.UserName
            });
        }

        public async Task<IEnumerable<Customer>> GetByBranchesAsync(int branchId)
        {
            var branchIds = _branchHelper.GetBranchsIDs(branchId);

            var entities = await _dbContext.tblCustomer
                .Where(c => branchIds.Contains(c.BranchID))
                .Include(c => c.Branch)
                .Include(c => c.Company)
                .Include(c => c.User)
                .ToListAsync();

            return entities.Select(c => new Customer
            {
                CustomerID = c.CustomerID,
                Customername = c.Customername,
                CustomerContact = c.CustomerContact,
                CustomerAddress = c.CustomerAddress,
                Description = c.Description,
                BranchID = c.BranchID,
                CompanyID = c.CompanyID,
                UserID = c.UserID,
                BranchName = c.Branch.BranchName,
                CompanyName = c.Company.Name,
                UserName = c.User.UserName
            });
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblCustomer
                .Include(c => c.Branch)
                .Include(c => c.Company)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.CustomerID == id);

            return entity == null ? null : new Customer
            {
                CustomerID = entity.CustomerID,
                Customername = entity.Customername,
                CustomerContact = entity.CustomerContact,
                CustomerAddress = entity.CustomerAddress,
                Description = entity.Description,
                BranchID = entity.BranchID,
                CompanyID = entity.CompanyID,
                UserID = entity.UserID,
                BranchName = entity.Branch.BranchName,
                CompanyName = entity.Company.Name,
                UserName = entity.User.UserName
            };
        }

        public async Task AddAsync(Customer customer)
        {
            var entity = new tblCustomer
            {
                Customername = customer.Customername,
                CustomerContact = customer.CustomerContact,
                CustomerAddress = customer.CustomerAddress,
                Description = customer.Description,
                BranchID = customer.BranchID,
                CompanyID = customer.CompanyID,
                UserID = customer.UserID
            };

            _dbContext.tblCustomer.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            var entity = await _dbContext.tblCustomer.FindAsync(customer.CustomerID);

            entity.Customername = customer.Customername;
            entity.CustomerContact = customer.CustomerContact;
            entity.CustomerAddress = customer.CustomerAddress;
            entity.Description = customer.Description;
            entity.BranchID = customer.BranchID;
            entity.CompanyID = customer.CompanyID;
            entity.UserID = customer.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsExists(Customer customer)
        {
            return await _dbContext.tblCustomer
                .AnyAsync(c => c.Customername == customer.Customername &&
                               c.CustomerID != customer.CustomerID &&
                               c.BranchID == customer.BranchID &&
                               c.CompanyID == customer.CompanyID);
        }
    }
}
