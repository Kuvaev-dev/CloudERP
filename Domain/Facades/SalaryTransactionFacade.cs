using Domain.RepositoryAccess;

namespace Domain.Facades
{
    public class SalaryTransactionFacade
    {
        private readonly ISalaryTransactionRepository _salaryTransactionRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IAccountSettingRepository _accountSettingRepository;

        public SalaryTransactionFacade(
            ISalaryTransactionRepository salaryTransactionRepository, 
            IEmployeeRepository employeeRepository, 
            IFinancialYearRepository financialYearRepository, 
            IAccountSettingRepository accountSettingRepository)
        {
            _salaryTransactionRepository = salaryTransactionRepository;
            _employeeRepository = employeeRepository;
            _financialYearRepository = financialYearRepository;
            _accountSettingRepository = accountSettingRepository;
        }

        public ISalaryTransactionRepository SalaryTransactionRepository => _salaryTransactionRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IFinancialYearRepository FinancialYearRepository => _financialYearRepository;
        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
    }
}
