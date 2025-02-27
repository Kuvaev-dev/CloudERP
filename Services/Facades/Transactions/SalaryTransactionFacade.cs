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
            _salaryTransactionRepository = salaryTransactionRepository ?? throw new ArgumentNullException(nameof(ISalaryTransactionRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(IAccountSettingRepository));
        }

        public ISalaryTransactionRepository SalaryTransactionRepository => _salaryTransactionRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IFinancialYearRepository FinancialYearRepository => _financialYearRepository;
        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
    }
}
