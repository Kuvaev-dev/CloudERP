using Domain.RepositoryAccess;

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
            _companyRepository = companyRepository;
            _branchRepository = branchRepository;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _emailService = emailService;

        }

        public ICompanyRepository CompanyRepository => _companyRepository;
        public IBranchRepository BranchRepository => _branchRepository;
        public IUserRepository UserRepository => _userRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public IEmailService EmailService => _emailService;
    }
}