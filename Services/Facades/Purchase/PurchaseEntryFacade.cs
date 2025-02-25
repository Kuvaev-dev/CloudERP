using Domain.RepositoryAccess;

namespace Services.Facades
{
    public class PurchaseEntryFacade
    {
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IPurchaseRepository _purchaseRepository;
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IStockRepository _stockRepository;
        private readonly IPurchaseCartDetailRepository _purchaseCartDetailRepository;

        public PurchaseEntryFacade(
            IAccountSettingRepository accountSettingRepository,
            IPurchaseRepository purchaseRepository,
            IFinancialYearRepository financialYearRepository,
            IStockRepository stockRepository,
            IPurchaseCartDetailRepository purchaseCartDetailRepository)
        {
            _accountSettingRepository = accountSettingRepository ?? throw new System.ArgumentNullException(nameof(accountSettingRepository));
            _purchaseRepository = purchaseRepository ?? throw new System.ArgumentNullException(nameof(purchaseRepository));
            _financialYearRepository = financialYearRepository ?? throw new System.ArgumentNullException(nameof(financialYearRepository));
            _stockRepository = stockRepository ?? throw new System.ArgumentNullException(nameof(stockRepository));
            _purchaseCartDetailRepository = purchaseCartDetailRepository ?? throw new System.ArgumentNullException(nameof(purchaseCartDetailRepository));
        }

        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
        public IPurchaseRepository PurchaseRepository => _purchaseRepository;
        public IFinancialYearRepository FinancialYearRepository => _financialYearRepository;
        public IStockRepository StockRepository => _stockRepository;
        public IPurchaseCartDetailRepository PurchaseCartDetailRepository => _purchaseCartDetailRepository;
    }
}
