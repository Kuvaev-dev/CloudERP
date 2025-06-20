﻿using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class EmployeeSalaryService : IEmployeeSalaryService
    {
        private readonly IPayrollRepository _payrollRepository;
        private readonly ISalaryTransactionService _salaryTransaction;

        public EmployeeSalaryService(IPayrollRepository payrollRepository, ISalaryTransactionService salaryTransaction)
        {
            _payrollRepository = payrollRepository ?? throw new ArgumentNullException(nameof(payrollRepository));
            _salaryTransaction = salaryTransaction ?? throw new ArgumentNullException(nameof(salaryTransaction));
        }

        public async Task<string> ConfirmSalaryAsync(Salary salary, int userId, int branchId, int companyId)
        {
            var emp = await _payrollRepository.GetEmployeePayrollAsync(
                salary.EmployeeID, branchId, companyId, salary.SalaryMonth.ToLower(), salary.SalaryYear);

            if (emp != null)
            {
                return Localization.Services.Localization.SalaryIsAlreadyPaid;
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
    }
}
