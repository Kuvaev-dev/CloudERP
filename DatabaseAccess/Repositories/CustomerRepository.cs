using DatabaseAccess.Helpers;
using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CloudDBEntities _dbContext;

        public CustomerRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            try
            {
                var entities = await _dbContext.tblCustomer
                .Include(c => c.tblBranch)
                .Include(c => c.tblCompany)
                .Include(c => c.tblUser)
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
                    BranchName = c.tblBranch.BranchName,
                    CompanyName = c.tblCompany.Name,
                    UserName = c.tblUser.UserName
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetAllAsync), ex);
                throw new InvalidOperationException("Error retrieving customers.", ex);
            }
        }

        public async Task<IEnumerable<Customer>> GetByCompanyAndBranchAsync(int companyId, int branchId)
        {
            try
            {
                var entities = await _dbContext.tblCustomer
                    .Where(c => c.CompanyID == companyId && c.BranchID == branchId)
                    .Include(c => c.tblBranch)
                    .Include(c => c.tblCompany)
                    .Include(c => c.tblUser)
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
                    BranchName = c.tblBranch.BranchName,
                    CompanyName = c.tblCompany.Name,
                    UserName = c.tblUser.UserName
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving customers.", ex);
            }
        }

        public async Task<IEnumerable<Customer>> GetByBranchesAsync(int branchId)
        {
            var branchIds = BranchHelper.GetBranchsIDs(branchId, _dbContext);
            try
            {
                var entities = await _dbContext.tblCustomer
                    .Where(c => branchIds.Contains(c.BranchID))
                    .Include(c => c.tblBranch)
                    .Include(c => c.tblCompany)
                    .Include(c => c.tblUser)
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
                    BranchName = c.tblBranch.BranchName,
                    CompanyName = c.tblCompany.Name,
                    UserName = c.tblUser.UserName
                });
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving customers.", ex);
            }
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _dbContext.tblCustomer
                    .Include(c => c.tblBranch)
                    .Include(c => c.tblCompany)
                    .Include(c => c.tblUser)
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
                    BranchName = entity.tblBranch.BranchName,
                    CompanyName = entity.tblCompany.Name,
                    UserName = entity.tblUser.UserName
                };
            }
            catch (Exception ex)
            {
                LogException(nameof(GetByIdAsync), ex);
                throw new InvalidOperationException($"Error retrieving customer with ID {id}.", ex);
            }
        }

        public async Task AddAsync(Customer customer)
        {
            try
            {
                if (customer == null) throw new ArgumentNullException(nameof(customer));

                var entity = new tblCustomer
                {
                    CustomerID = customer.CustomerID,
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
            catch (Exception ex)
            {
                LogException(nameof(AddAsync), ex);
                throw new InvalidOperationException("Error adding a new customer.", ex);
            }
        }

        public async Task UpdateAsync(Customer customer)
        {
            try
            {
                if (customer == null) throw new ArgumentNullException(nameof(customer));

                var entity = await _dbContext.tblCustomer.FindAsync(customer.CustomerID);
                if (entity == null) throw new KeyNotFoundException("Customer not found.");

                entity.CustomerID = customer.CustomerID;
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
            catch (KeyNotFoundException ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw;
            }
            catch (Exception ex)
            {
                LogException(nameof(UpdateAsync), ex);
                throw new InvalidOperationException($"Error updating customer with ID {customer.CustomerID}.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
