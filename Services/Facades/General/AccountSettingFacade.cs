using Domain.RepositoryAccess;

namespace Services.Facades
{
    public class AccountSettingFacade
    {
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IAccountControlRepository _accountControlRepository;
        private readonly IAccountSubControlRepository _accountSubControlRepository;
        private readonly IAccountHeadRepository _accountHeadRepository;
        private readonly IAccountActivityRepository _accountActivityRepository;

        public AccountSettingFacade(
            IAccountSettingRepository accountSettingRepository,
            IAccountControlRepository accountControlRepository,
            IAccountSubControlRepository accountSubControlRepository,
            IAccountHeadRepository accountHeadRepository,
            IAccountActivityRepository accountActivityRepository)
        {
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(IAccountSettingRepository));
            _accountControlRepository = accountControlRepository ?? throw new ArgumentNullException(nameof(IAccountControlRepository));
            _accountSubControlRepository = accountSubControlRepository ?? throw new ArgumentNullException(nameof(IAccountSubControlRepository));
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(IAccountHeadRepository));
            _accountActivityRepository = accountActivityRepository ?? throw new ArgumentNullException(nameof(IAccountActivityRepository));
        }

        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
        public IAccountControlRepository AccountControlRepository => _accountControlRepository;
        public IAccountSubControlRepository AccountSubControlRepository => _accountSubControlRepository;
        public IAccountHeadRepository AccountHeadRepository => _accountHeadRepository;
        public IAccountActivityRepository AccountActivityRepository => _accountActivityRepository;
    }
}