using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Facades
{
    public class HomeFacade
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly IAuthService _authService;
        private readonly IDashboardService _dashboardService;

        public HomeFacade(
            IDashboardService dashboardService,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            ICompanyRepository companyRepository,
            IAuthService authService)
        {
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(dashboardService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(companyRepository));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public IDashboardService DashboardService => _dashboardService;
        public IUserRepository UserRepository => _userRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public ICompanyRepository CompanyRepository => _companyRepository;
        public IAuthService AuthService => _authService;
    }
}