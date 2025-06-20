using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Facades
{
    public class CompanyEmployeeFacade
    {
        private readonly ISalaryTransactionService _salaryTransactionService;
        private readonly IEmployeeSalaryService _employeeSalaryService;
        private readonly IEmailService _emailService;
        private readonly IFileService _fileService;
        private readonly IFileAdapterFactory _fileAdapterFactory;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IPayrollRepository _payrollRepository;

        public CompanyEmployeeFacade(
            ISalaryTransactionService salaryTransactionService,
            IEmployeeSalaryService employeeSalaryService,
            IEmailService emailService,
            IFileService fileService,
            IFileAdapterFactory fileAdapterFactory,
            IEmployeeRepository employeeRepository,
            IBranchRepository branchRepository,
            IPayrollRepository payrollRepository)
        {
            _salaryTransactionService = salaryTransactionService ?? throw new ArgumentNullException(nameof(salaryTransactionService));
            _employeeSalaryService = employeeSalaryService ?? throw new ArgumentNullException(nameof(employeeSalaryService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _fileAdapterFactory = fileAdapterFactory ?? throw new ArgumentNullException(nameof(fileAdapterFactory));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
            _payrollRepository = payrollRepository ?? throw new ArgumentNullException(nameof(payrollRepository));
        }

        public ISalaryTransactionService SalaryTransactionService => _salaryTransactionService;
        public IEmployeeSalaryService EmployeeSalaryService => _employeeSalaryService;
        public IEmailService EmailService => _emailService;
        public IFileService FileService => _fileService;
        public IFileAdapterFactory FileAdapterFactory => _fileAdapterFactory;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IBranchRepository BranchRepository => _branchRepository;
        public IPayrollRepository PayrollRepository => _payrollRepository;
    }
}