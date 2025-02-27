using Domain.RepositoryAccess;

namespace Domain.Facades
{
    public class SaleEntryFacade
    {
        private readonly IFinancialYearRepository _financialYearRepository;
        private readonly IAccountSettingRepository _accountSettingRepository;
        private readonly IStockRepository _stockRepository;
        private readonly ISaleCartDetailRepository _saleCartDetailRepository;
        private readonly ISaleRepository _saleRepository;

        public SaleEntryFacade(
            IFinancialYearRepository financialYearRepository, 
            IAccountSettingRepository accountSettingRepository, 
            ISaleRepository saleRepository, 
            IStockRepository stockRepository, 
            ISaleCartDetailRepository saleCartDetailRepository)
        {
            _financialYearRepository = financialYearRepository ?? throw new ArgumentNullException(nameof(IFinancialYearRepository));
            _accountSettingRepository = accountSettingRepository ?? throw new ArgumentNullException(nameof(IAccountSettingRepository));
            _saleRepository = saleRepository ?? throw new ArgumentNullException(nameof(ISaleRepository));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(IStockRepository));
            _saleCartDetailRepository = saleCartDetailRepository ?? throw new ArgumentNullException(nameof(ISaleCartDetailRepository));
        }

        public IFinancialYearRepository FinancialYearRepository => _financialYearRepository;
        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
        public ISaleRepository SaleRepository => _saleRepository;
        public IStockRepository StockRepository => _stockRepository;
        public ISaleCartDetailRepository SaleCartDetailRepository => _saleCartDetailRepository;
    }
}
