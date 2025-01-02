using Domain.RepositoryAccess;
using Domain.Services.Interfaces;
using System;

namespace CloudERP.Facades
{
    public class CompanyRegistrationFacade
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IEmailService _emailService;

        public CompanyRegistrationFacade(
            ICompanyRepository companyRepository,
            IBranchRepository branchRepository,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IEmailService emailService)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(ICompanyRepository));
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(IBranchRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(IEmailService));
        }

        public ICompanyRepository CompanyRepository => _companyRepository;
        public IBranchRepository BranchRepository => _branchRepository;
        public IUserRepository UserRepository => _userRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IEmailService EmailService => _emailService;
    }
}