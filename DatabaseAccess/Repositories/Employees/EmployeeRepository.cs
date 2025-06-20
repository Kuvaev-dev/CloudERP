using DatabaseAccess.Context;
using DatabaseAccess.Models;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Employees
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
                .Include(e => e.Branch)
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
                BranchTypeID = e.Branch.BranchTypeID,
                BrchID = e.Branch.BrchID,
                BranchName = e.Branch.BranchName,
                UserID = e.UserID
            });
        }

        public async Task<IEnumerable<Employee>> GetEmployeesForTaskAssignmentAsync(int branchId, int companyId)
        {
            var employees = await _dbContext.tblEmployee
                .Where(e => e.Branch.BrchID == branchId
                    && e.Branch.BranchTypeID > 1
                    && e.Company.CompanyID == companyId)
                .ToListAsync();

            return employees.Select(e => new Employee
            {
                EmployeeID = e.EmployeeID,
                FullName = e.Name,
                BranchID = e.BranchID,
                CompanyID = e.CompanyID,
                UserID = e.UserID
            });
        }

        public async Task<Employee?> GetByContactAsync(string contact)
        {
            var e = await _dbContext.tblEmployee
                .Include(e => e.Branch)
                .FirstOrDefaultAsync(e => e.ContactNo == contact);

            return e == null ? null : new Employee
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
                BranchTypeID = e.Branch.BranchTypeID,
                BrchID = e.Branch.BrchID,
                BranchName = e.Branch.BranchName,
                UserID = e.UserID
            };
        }

        public async Task<IEnumerable<Employee>> GetByCompanyIdAsync(int companyId)
        {
            var entities = await _dbContext.tblEmployee
                .Include(e => e.Branch)
                .Where(e => e.CompanyID == companyId)
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
                BranchTypeID = e.Branch.BranchTypeID,
                BrchID = e.Branch.BrchID,
                BranchName = e.Branch.BranchName,
                UserID = e.UserID
            });
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblEmployee
                .Include(e => e.Branch)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

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
                BranchTypeID = entity.Branch.BranchTypeID,
                BrchID = entity.Branch.BrchID,
                BranchName = entity.Branch.BranchName,
                UserID = entity.UserID
            };
        }

        public async Task AddAsync(Employee employee)
        {
            var entity = new tblEmployee
            {
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
            employee.EmployeeID = entity.EmployeeID;
        }

        public async Task UpdateAsync(Employee employee)
        {
            var entity = await _dbContext.tblEmployee.FindAsync(employee.EmployeeID);

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
            entity.UserID = employee.UserID;

            _dbContext.tblEmployee.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Employee?> GetByUserIdAsync(int id)
        {
            var entity = await _dbContext.tblEmployee
                .Include(b => b.Branch)
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
                BranchTypeID = entity.Branch.BranchTypeID,
                BrchID = entity.Branch.BrchID,
                BranchName = entity.Branch.BranchName,
                UserID = entity.UserID
            };
        }

        public async Task<bool> IsFirstLoginAsync(Employee employee)
        {
            var employeeRecord = await _dbContext.tblEmployee.FirstOrDefaultAsync(e => e.EmployeeID == employee.EmployeeID);

            if (employeeRecord?.IsFirstLogin == true)
            {
                employeeRecord.IsFirstLogin = false;
                await _dbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDateRangeAsync(DateTime startDate, DateTime endDate, List<int> branchIDs, int companyID)
        {
            var entities = await _dbContext.tblEmployee
                .Include(e => e.Branch)
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
                BranchTypeID = e.Branch.BranchTypeID,
                BrchID = e.Branch.BrchID,
                BranchName = e.Branch.BranchName,
                UserID = e.UserID
            });
        }

        public async Task<Employee?> GetByTINAsync(string TIN)
        {
            var entity = await _dbContext.tblEmployee
                .Include(e => e.Branch)
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
                BranchTypeID = entity.Branch.BranchTypeID,
                BrchID = entity.Branch.BrchID,
                BranchName = entity.Branch.BranchName,
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
                         && e.RegistrationDate.Value.Year == DateTime.Now.Year
                         && e.RegistrationDate.Value.Month == DateTime.Now.Month)
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

        public async Task<bool> IsExists(Employee employee)
        {
            return await _dbContext.tblEmployee
                .AnyAsync(e => e.Name == employee.FullName
                            && e.ContactNo == employee.ContactNumber
                            && e.Email == employee.Email
                            && e.TIN == employee.TIN
                            && e.EmployeeID != employee.EmployeeID);
        }
    }
}
