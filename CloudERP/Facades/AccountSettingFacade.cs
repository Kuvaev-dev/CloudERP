using Domain.RepositoryAccess;

namespace CloudERP.Facades
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
            _accountSettingRepository = accountSettingRepository;
            _accountControlRepository = accountControlRepository;
            _accountSubControlRepository = accountSubControlRepository;
            _accountHeadRepository = accountHeadRepository;
            _accountActivityRepository = accountActivityRepository;
        }

        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
        public IAccountControlRepository AccountControlRepository => _accountControlRepository;
        public IAccountSubControlRepository AccountSubControlRepository => _accountSubControlRepository;
        public IAccountHeadRepository AccountHeadRepository => _accountHeadRepository;
        public IAccountActivityRepository AccountActivityRepository => _accountActivityRepository;
    }
}