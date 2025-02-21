using DatabaseAccess.Context;
using Domain.Models;
using Domain.RepositoryAccess;
using Microsoft.EntityFrameworkCore;

namespace DatabaseAccess.Repositories.Employees
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly CloudDBEntities _dbContext;

        public PayrollRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(CloudDBEntities));
        }

        public async Task<Payroll?> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblPayroll.FirstOrDefaultAsync(p => p.PayrollID == id);

            return entity == null ? null : new Payroll
            {
                PayrollID = entity.PayrollID,
                EmployeeID = entity.EmployeeID,
                EmployeeName = entity.Employee.Name,
                BranchID = entity.BranchID,
                BranchName = entity.Branch.BranchName,
                BranchAddress = entity.Branch.BranchAddress,
                BranchContact = entity.Branch.BranchContact,
                CompanyID = entity.CompanyID,
                CompanyName = entity.Company.Name,
                CompanyLogo = entity.Company.Logo,
                TransferAmount = entity.TransferAmount,
                PayrollInvoiceNo = entity.PayrollInvoiceNo,
                PaymentDate = entity.PaymentDate,
                SalaryMonth = entity.SalaryMonth,
                SalaryYear = entity.SalaryYear,
                UserID = entity.UserID,
                UserName = entity.User.UserName
            };
        }

        public async Task<Payroll?> GetEmployeePayrollAsync(int employeeID, int branchID, int companyID, string salaryMonth, string salaryYear)
        {
            var entity = await _dbContext.tblPayroll.FirstOrDefaultAsync(p =>
                p.EmployeeID == employeeID &&
                p.BranchID == branchID &&
                p.CompanyID == companyID &&
                p.SalaryMonth == salaryMonth &&
                p.SalaryYear == salaryYear);

            return entity == null ? null : new Payroll
            {
                PayrollID = entity.PayrollID,
                EmployeeID = entity.EmployeeID,
                BranchID = entity.BranchID,
                CompanyID = entity.CompanyID,
                TransferAmount = entity.TransferAmount,
                PayrollInvoiceNo = entity.PayrollInvoiceNo,
                PaymentDate = entity.PaymentDate,
                SalaryMonth = entity.SalaryMonth,
                SalaryYear = entity.SalaryYear,
                UserID = entity.UserID
            };
        }

        public async Task<int> GetLatestPayrollIdAsync()
        {
            return await _dbContext.tblPayroll.MaxAsync(p => p.PayrollID);
        }

        public async Task<Payroll?> GetLatestPayrollAsync()
        {
            var payrollID = await GetLatestPayrollIdAsync();
            var payroll = await _dbContext.tblPayroll.FirstOrDefaultAsync(p => p.PayrollID == payrollID);

            return payroll == null ? null : new Payroll
            {
                PayrollID = payroll.PayrollID,
                EmployeeID = payroll.EmployeeID,
                BranchID = payroll.BranchID,
                CompanyID = payroll.CompanyID,
                TransferAmount = payroll.TransferAmount,
                PayrollInvoiceNo = payroll.PayrollInvoiceNo,
                PaymentDate = payroll.PaymentDate,
                SalaryMonth = payroll.SalaryMonth,
                SalaryYear = payroll.SalaryYear,
                UserID = payroll.UserID
            };
        }

        public async Task<IEnumerable<Payroll>> GetSalaryHistoryAsync(int branchID, int companyID)
        {
            var entities = await _dbContext.tblPayroll
                .Where(p => p.BranchID == branchID && p.CompanyID == companyID)
                .OrderByDescending(p => p.PayrollID)
                .ToListAsync();

            return entities.Select(p => new Payroll
            {
                PayrollID = p.PayrollID,
                EmployeeID = p.EmployeeID,
                EmployeeName = p.Employee.Name,
                BranchID = p.BranchID,
                CompanyID = p.CompanyID,
                TransferAmount = p.TransferAmount,
                PayrollInvoiceNo = p.PayrollInvoiceNo,
                PaymentDate = p.PaymentDate,
                SalaryMonth = p.SalaryMonth,
                SalaryYear = p.SalaryYear,
                UserID = p.UserID,
                UserName = p.User.UserName
            });
        }
    }
}
