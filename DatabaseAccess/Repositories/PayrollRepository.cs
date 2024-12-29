using Domain.Models;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DatabaseAccess.Repositories
{
    public class PayrollRepository : IPayrollRepository
    {
        private readonly CloudDBEntities _dbContext;

        public PayrollRepository(CloudDBEntities dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Payroll> GetByIdAsync(int id)
        {
            var entity = await _dbContext.tblPayroll.FirstOrDefaultAsync(p => p.PayrollID == id);

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

        public async Task<Payroll> GetEmployeePayrollAsync(int employeeID, int branchID, int companyID, string salaryMonth, string salaryYear)
        {
            var entity = await _dbContext.tblPayroll.FirstOrDefaultAsync(p => p.EmployeeID == employeeID &&
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

        public async Task<int> GetLatestPayrollAsync()
        {
            return await _dbContext.tblPayroll.MaxAsync(p => p.PayrollID);
        }

        public async Task<IEnumerable<Payroll>> GetSalaryHistoryAsync(int branchID, int companyID)
        {
            var entities = await _dbContext.tblPayroll.Where(p => p.BranchID == branchID && p.CompanyID == companyID)
                                            .OrderByDescending(p => p.PayrollID)
                                            .ToListAsync();

            return entities.Select(p => new Payroll
            {
                PayrollID = p.PayrollID,
                EmployeeID = p.EmployeeID,
                BranchID = p.BranchID,
                CompanyID = p.CompanyID,
                TransferAmount = p.TransferAmount,
                PayrollInvoiceNo = p.PayrollInvoiceNo,
                PaymentDate = p.PaymentDate,
                SalaryMonth = p.SalaryMonth,
                SalaryYear = p.SalaryYear,
                UserID = p.UserID
            });
        }
    }
}
