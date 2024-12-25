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
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetEmployeePayrollAsync), ex);
                throw new InvalidOperationException($"Error retrieving employee payroll.", ex);
            }
        }

        public async Task<Payroll> GetEmployeePayrollAsync(int employeeID, int branchID, int companyID, string salaryMonth, string salaryYear)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetEmployeePayrollAsync), ex);
                throw new InvalidOperationException($"Error retrieving employee payroll.", ex);
            }
        }

        public async Task<int> GetLatestPayrollAsync()
        {
            try
            {
                return await _dbContext.tblPayroll.MaxAsync(p => p.PayrollID);
            }
            catch (Exception ex)
            {
                LogException(nameof(GetEmployeePayrollAsync), ex);
                throw new InvalidOperationException($"Error retrieving employee payroll.", ex);
            }
        }

        public async Task<IEnumerable<Payroll>> GetSalaryHistoryAsync(int branchID, int companyID)
        {
            try
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
            catch (Exception ex)
            {
                LogException(nameof(GetEmployeePayrollAsync), ex);
                throw new InvalidOperationException($"Error retrieving employee payroll.", ex);
            }
        }

        private void LogException(string methodName, Exception ex)
        {
            Console.WriteLine($"Error in {methodName}: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
