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
            _salaryTransactionRepository = salaryTransactionRepository ?? throw new ArgumentNullException(nameof(salaryTransactionRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(financialYearRepository));
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(accountSettingRepository));
        }

        public ISalaryTransactionRepository SalaryTransactionRepository => _salaryTransactionRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IFinancialYearRepository FinancialYearRepository => _financialYearRepository;
        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
    }
}
