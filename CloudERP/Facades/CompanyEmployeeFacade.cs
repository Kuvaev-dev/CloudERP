using DatabaseAccess.Factories;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using System;
using Utils.Interfaces;

namespace CloudERP.Facades
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
            IEmployeeRepository employeeRepository,
            IBranchRepository branchRepository,
            IPayrollRepository payrollRepository,
            IFileAdapterFactory fileAdapterFactory)
        {
            _salaryTransactionService = salaryTransactionService ?? throw new ArgumentNullException(nameof(ISalaryTransactionService));
            _employeeSalaryService = employeeSalaryService ?? throw new ArgumentNullException(nameof(IEmployeeSalaryService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(IEmailService));
            _fileService = fileService ?? throw new ArgumentNullException(nameof(IFileService));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(IBranchRepository));
            _payrollRepository = payrollRepository ?? throw new ArgumentNullException(nameof(IPayrollRepository));
            _fileAdapterFactory = fileAdapterFactory ?? throw new ArgumentNullException(nameof(IFileAdapterFactory));
        }

        public ISalaryTransactionService SalaryTransactionService => _salaryTransactionService;
        public IEmployeeSalaryService EmployeeSalaryService => _employeeSalaryService;
        public IEmailService EmailService => _emailService;
        public IFileService FileService => _fileService;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IBranchRepository BranchRepository => _branchRepository;
        public IPayrollRepository PayrollRepository => _payrollRepository;
        public IFileAdapterFactory FileAdapterFactory => _fileAdapterFactory;
    }
}