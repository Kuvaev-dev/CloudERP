using Domain.RepositoryAccess;

namespace CloudERP.Facades
{
    public class HomeFacade
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICompanyRepository _companyRepository;

        public HomeFacade(
            IDashboardRepository dashboardRepository,
            IUserRepository userRepository,
            IEmployeeRepository employeeRepository,
            ICompanyRepository companyRepository)
        {
            _dashboardRepository = dashboardRepository;
            _userRepository = userRepository;
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
        }

        public IDashboardRepository DashboardRepository => _dashboardRepository;
        public IUserRepository UserRepository => _userRepository;
        public IEmployeeRepository EmployeeRepository => _employeeRepository;
        public ICompanyRepository CompanyRepository => _companyRepository;
    }
}