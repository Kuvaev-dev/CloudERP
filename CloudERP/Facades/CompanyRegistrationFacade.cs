using Domain.Interfaces;
using Domain.RepositoryAccess;
using System;

namespace CloudERP.Facades
{
    public class CompanyRegistrationFacade
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IEmailService _emailService;

        public CompanyRegistrationFacade(
            ICompanyRepository companyRepository,
            IBranchRepository branchRepository,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IAccountSettingRepository accountSettingRepository,
            IEmailService emailService)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(ICompanyRepository));
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(IBranchRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(IAccountSettingRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(IEmailService));
        }

        public ICompanyRepository CompanyRepository => _companyRepository;
        public IBranchRepository BranchRepository => _branchRepository;
        public IUserRepository UserRepository => _userRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
        public IEmailService EmailService => _emailService;
    }
}