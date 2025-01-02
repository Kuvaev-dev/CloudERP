using Domain.RepositoryAccess;
using Domain.Services;

namespace CloudERP.Facades
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
            _dashboardService = dashboardService;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _authService = authService;
        }

        public IDashboardService DashboardService => _dashboardService;
        public IUserRepository UserRepository => _userRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public ICompanyRepository CompanyRepository => _companyRepository;
        public IAuthService AuthService => _authService;
    }
}