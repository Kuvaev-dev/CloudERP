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
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(accountSettingRepository));
            _accountControlRepository = accountControlRepository ?? throw new ArgumentNullException(nameof(accountControlRepository));
            _accountSubControlRepository = accountSubControlRepository ?? throw new ArgumentNullException(nameof(accountSubControlRepository));
            _accountHeadRepository = accountHeadRepository ?? throw new ArgumentNullException(nameof(accountHeadRepository));
            _accountActivityRepository = accountActivityRepository ?? throw new ArgumentNullException(nameof(accountActivityRepository));
        }

        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
        public IAccountControlRepository AccountControlRepository => _accountControlRepository;
        public IAccountSubControlRepository AccountSubControlRepository => _accountSubControlRepository;
        public IAccountHeadRepository AccountHeadRepository => _accountHeadRepository;
        public IAccountActivityRepository AccountActivityRepository => _accountActivityRepository;
    }
}