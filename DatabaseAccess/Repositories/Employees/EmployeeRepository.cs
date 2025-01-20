using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly CloudDBEntities _dbContext;

        public EmployeeRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<IEnumerable<Employee>> GetByBranchAsync(int companyId, int branchId)
        {
            var entities = await _dbContext.tblEmployee
                .Where(e => e.CompanyID == companyId && e.BranchID == branchId)
                .ToListAsync();

            return entities.Select(e => new Employee
            {
                EmployeeID = e.EmployeeID,
                FullName = e.Name,
                ContactNumber = e.ContactNo,
                Email = e.Email,
                Address = e.Address,
                Photo = e.Photo,
                TIN = e.TIN,
                Designation = e.Designation,
                Description = e.Description,
                MonthlySalary = e.MonthlySalary,
                IsFirstLogin = e.IsFirstLogin,
                RegistrationDate = e.RegistrationDate,
                CompanyID = e.CompanyID,
                BranchID = e.BranchID,
                BranchTypeID = e.tblBranch.BranchTypeID,
                BrchID = e.tblBranch.BrchID,
                BranchName = e.tblBranch.BranchName,
                UserID = e.UserID
            });
        }

        public async Task<IEnumerable<Employee>> GetEmployeesForTaskAssignmentAsync(int branchId)
        {
            var employees = await _dbContext.tblEmployee
                .Where(e => e.tblBranch.BranchID == branchId && e.tblBranch.BranchTypeID == 2)
                .ToListAsync();

            return employees.Select(e => new Employee
            {
                EmployeeID = e.EmployeeID,
                FullName = e.Name,
                BranchID = e.BranchID,
                CompanyID = e.CompanyID
            });
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblEmployee.FindAsync(id);

            return entity == null ? null : new Employee
            {
                EmployeeID = entity.EmployeeID,
                FullName = entity.Name,
                ContactNumber = entity.ContactNo,
                Email = entity.Email,
                Address = entity.Address,
                Photo = entity.Photo,
                TIN = entity.TIN,
                Designation = entity.Designation,
                Description = entity.Description,
                MonthlySalary = entity.MonthlySalary,
                IsFirstLogin = entity.IsFirstLogin,
                RegistrationDate = entity.RegistrationDate,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                BranchTypeID = entity.tblBranch.BranchTypeID,
                BrchID = entity.tblBranch.BrchID,
                BranchName = entity.tblBranch.BranchName,
                UserID = entity.UserID
            };
        }

        public async Task AddAsync(Employee employee)
        {
            var entity = new tblEmployee
            {
                EmployeeID = employee.EmployeeID,
                Name = employee.FullName,
                ContactNo = employee.ContactNumber,
                Email = employee.Email,
                Address = employee.Address,
                Photo = employee.Photo,
                TIN = employee.TIN,
                Designation = employee.Designation,
                Description = employee.Description,
                MonthlySalary = employee.MonthlySalary,
                IsFirstLogin = employee.IsFirstLogin,
                RegistrationDate = employee.RegistrationDate,
                CompanyID = employee.CompanyID,
                BranchID = employee.BranchID,
                UserID = employee.UserID
            };

            _dbContext.tblEmployee.Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            var entity = await _dbContext.tblEmployee.FindAsync(employee.EmployeeID);

            entity.EmployeeID = employee.EmployeeID;
            entity.Name = employee.FullName;
            entity.ContactNo = employee.ContactNumber;
            entity.Email = employee.Email;
            entity.Address = employee.Address;
            entity.Photo = employee.Photo;
            entity.TIN = employee.TIN;
            entity.Designation = employee.Designation;
            entity.Description = employee.Description;
            entity.MonthlySalary = employee.MonthlySalary;
            entity.IsFirstLogin = employee.IsFirstLogin;
            entity.RegistrationDate = employee.RegistrationDate;
            entity.CompanyID = employee.CompanyID;
            entity.BranchID = employee.BranchID;
            entity.UserID = employee.UserID;

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Employee> GetByUserIdAsync(int id)
        {
            var entity = await _dbContext.tblEmployee
                .Include(b => b.tblBranch)
                .Where(e => e.UserID == id)
                .FirstOrDefaultAsync();

            return entity == null ? null : new Employee
            {
                EmployeeID = entity.EmployeeID,
                FullName = entity.Name,
                ContactNumber = entity.ContactNo,
                Email = entity.Email,
                Address = entity.Address,
                Photo = entity.Photo,
                TIN = entity.TIN,
                Designation = entity.Designation,
                Description = entity.Description,
                MonthlySalary = entity.MonthlySalary,
                IsFirstLogin = entity.IsFirstLogin,
                RegistrationDate = entity.RegistrationDate,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                BranchTypeID = entity.tblBranch.BranchTypeID,
                BrchID = entity.tblBranch.BrchID,
                BranchName = entity.tblBranch.BranchName,
                UserID = entity.UserID
            };
        }

        public async Task<bool> IsFirstLoginAsync(Employee employee)
        {
            var employeeRecord = await _dbContext.tblEmployee.FirstOrDefaultAsync(e => e.EmployeeID == employee.EmployeeID);

            if (employeeRecord.IsFirstLogin == true)
            {
                employeeRecord.IsFirstLogin = false;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDateRangeAsync(DateTime startDate, DateTime endDate, List<int?> branchIDs, int companyID)
        {
            var entities = await _dbContext.tblEmployee
                .Where(e => e.RegistrationDate.HasValue
                    && e.RegistrationDate.Value >= startDate
                    && e.RegistrationDate.Value <= endDate
                    && e.CompanyID == companyID
                    && branchIDs.Contains(e.BranchID))
                .ToListAsync();

            return entities.Select(e => new Employee
            {
                EmployeeID = e.EmployeeID,
                FullName = e.Name,
                ContactNumber = e.ContactNo,
                Email = e.Email,
                Address = e.Address,
                Photo = e.Photo,
                TIN = e.TIN,
                Designation = e.Designation,
                Description = e.Description,
                MonthlySalary = e.MonthlySalary,
                IsFirstLogin = e.IsFirstLogin,
                RegistrationDate = e.RegistrationDate,
                CompanyID = e.CompanyID,
                BranchID = e.BranchID,
                BranchTypeID = e.tblBranch.BranchTypeID,
                BrchID = e.tblBranch.BrchID,
                BranchName = e.tblBranch.BranchName,
                UserID = e.UserID
            });
        }

        public async Task<Employee> GetByCompanyIdAsync(int id)
        {
            var entity = await _dbContext.tblEmployee
                .Where(e => e.CompanyID == id)
                .FirstOrDefaultAsync();

            return entity == null ? null : new Employee
            {
                EmployeeID = entity.EmployeeID,
                FullName = entity.Name,
                ContactNumber = entity.ContactNo,
                Email = entity.Email,
                Address = entity.Address,
                Photo = entity.Photo,
                TIN = entity.TIN,
                Designation = entity.Designation,
                Description = entity.Description,
                MonthlySalary = entity.MonthlySalary,
                IsFirstLogin = entity.IsFirstLogin,
                RegistrationDate = entity.RegistrationDate,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                BranchTypeID = entity.tblBranch.BranchTypeID,
                BrchID = entity.tblBranch.BrchID,
                BranchName = entity.tblBranch.BranchName,
                UserID = entity.UserID
            };
        }

        public async Task<Employee> GetByTINAsync(string TIN)
        {
            var entity = await _dbContext.tblEmployee
                .Where(e => e.TIN == TIN)
                .FirstOrDefaultAsync();

            return entity == null ? null : new Employee
            {
                EmployeeID = entity.EmployeeID,
                FullName = entity.Name,
                ContactNumber = entity.ContactNo,
                Email = entity.Email,
                Address = entity.Address,
                Photo = entity.Photo,
                TIN = entity.TIN,
                Designation = entity.Designation,
                Description = entity.Description,
                MonthlySalary = entity.MonthlySalary,
                IsFirstLogin = entity.IsFirstLogin,
                RegistrationDate = entity.RegistrationDate,
                CompanyID = entity.CompanyID,
                BranchID = entity.BranchID,
                BranchTypeID = entity.tblBranch.BranchTypeID,
                BrchID = entity.tblBranch.BrchID,
                BranchName = entity.tblBranch.BranchName,
                UserID = entity.UserID
            };
        }

        public Task<int> GetCountByCompanyAsync(int companyId)
        {
            return _dbContext.tblEmployee
                .Where(e => e.CompanyID == companyId)
                .CountAsync();
        }

        public Task<int> GetMonthNewEmployeesCountByCompanyAsync(int companyId)
        {
            return _dbContext.tblEmployee
                .Where(e => e.CompanyID == companyId 
                         && e.RegistrationDate.HasValue 
                         && e.RegistrationDate.Value.Date == DateTime.Now.Date)
                .CountAsync();
        }

        public Task<int> GetYearNewEmployeesCountByCompanyAsync(int companyId)
        {
            return _dbContext.tblEmployee
                .Where(e => e.CompanyID == companyId
                         && e.RegistrationDate.HasValue
                         && e.RegistrationDate.Value.Year == DateTime.Now.Year)
                .CountAsync();
        }
    }
}
