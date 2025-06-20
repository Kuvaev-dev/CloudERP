using Domain.RepositoryAccess;
using Domain.ServiceAccess;
using Domain.UtilsAccess;

namespace Services.Facades
{
    public class CompanyRegistrationFacade
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IEmailService _emailService;
        private readonly IPasswordHelper _passwordHelper;

        public CompanyRegistrationFacade(
            ICompanyRepository companyRepository,
            IBranchRepository branchRepository,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            IAccountSettingRepository accountSettingRepository,
            IEmailService emailService,
            IPasswordHelper passwordHelper)
        {
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _branchRepository = branchRepository ?? throw new ArgumentNullException(nameof(branchRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(accountSettingRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _passwordHelper = passwordHelper ?? throw new ArgumentNullException(nameof(passwordHelper));
        }

        public ICompanyRepository CompanyRepository => _companyRepository;
        public IBranchRepository BranchRepository => _branchRepository;
        public IUserRepository UserRepository => _userRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
        public IEmailService EmailService => _emailService;
        public IPasswordHelper PasswordHelper => _passwordHelper;
    }
}