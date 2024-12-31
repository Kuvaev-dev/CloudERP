using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using System;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IEmployeeSalaryService
    {
        Task<string> ConfirmSalaryAsync(Salary salary, int userId, int branchId, int companyId);
        Task<int> GetLatestPayrollNumberAsync();
    }

    public class EmployeeSalaryService : IEmployeeSalaryService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly SalaryTransactionService _salaryTransaction;

        public EmployeeSalaryService(IPayrollRepository payrollRepository, SalaryTransactionService salaryTransaction)
        {
            _payrollRepository = payrollRepository;
            _salaryTransaction = salaryTransaction;
        }

        public async Task<string> ConfirmSalaryAsync(Salary salary, int userId, int branchId, int companyId)
        {
            var emp = await _payrollRepository.GetEmployeePayrollAsync(
                salary.EmployeeID, branchId, companyId, salary.SalaryMonth.ToLower(), salary.SalaryYear);

            if (emp != null)
            {
                return Localization.Localization.SalaryIsAlreadyPaid;
            }

            string invoiceNo = $"ESA{DateTime.Now:yyyyMMddHHmmss}{DateTime.Now.Millisecond}";

            return await _salaryTransaction.Confirm(
                salary.EmployeeID,
                salary.TransferAmount,
                userId,
                branchId,
                companyId,
                invoiceNo,
                salary.SalaryMonth.ToLower(),
                salary.SalaryYear);
        }

        public async Task<int> GetLatestPayrollNumberAsync()
        {
            return await _payrollRepository.GetLatestPayrollAsync();
        }
    }
}
