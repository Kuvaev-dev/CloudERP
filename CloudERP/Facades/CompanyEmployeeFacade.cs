using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Facades
{
    public class CompanyEmployeeFacade
    {
        private readonly ISalaryTransactionService _salaryTransactionService;
        private readonly IEmployeeSalaryService _employeeSalaryService;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IPayrollRepository _payrollRepository;

        public CompanyEmployeeFacade(
            ISalaryTransactionService salaryTransactionService,
            IEmployeeSalaryService employeeSalaryService,
            IEmailService emailService,
            IFileService fileService,
            IEmployeeRepository employeeRepository,
            IBranchRepository branchRepository,
            IPayrollRepository payrollRepository)
        {
            _salaryTransactionService = salaryTransactionService;
            _employeeSalaryService = employeeSalaryService;
            _emailService = emailService;
            _fileService = fileService;
            _employeeRepository = employeeRepository;
            _branchRepository = branchRepository;
            _payrollRepository = payrollRepository;
        }

        public ISalaryTransactionService SalaryTransactionService => _salaryTransactionService;
        public IEmployeeSalaryService EmployeeSalaryService => _employeeSalaryService;
        public IEmailService EmailService => _emailService;
        public IFileService FileService => _fileService;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IBranchRepository BranchRepository => _branchRepository;
        public IPayrollRepository PayrollRepository => _payrollRepository;
    }
}