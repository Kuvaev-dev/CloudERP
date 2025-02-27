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
        private readonly ICurrencyService _currencyService;

        public HomeFacade(
            IDashboardService dashboardService,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            ICompanyRepository companyRepository,
            IAuthService authService,
            ICurrencyService currencyService)
        {
            _dashboardService = dashboardService ?? throw new ArgumentNullException(nameof(IDashboardService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
            _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(IEmployeeRepository));
            _companyRepository = companyRepository ?? throw new ArgumentNullException(nameof(ICompanyRepository));
            _authService = authService ?? throw new ArgumentNullException(nameof(IAuthService));
            _currencyService = currencyService;
        }

        public IDashboardService DashboardService => _dashboardService;
        public IUserRepository UserRepository => _userRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public ICompanyRepository CompanyRepository => _companyRepository;
        public IAuthService AuthService => _authService;
        public ICurrencyService CurrencyService => _currencyService;
    }
}