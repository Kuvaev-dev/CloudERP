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
            _financialYearRepository = financialYearRepository;
            _accountSettingRepository = accountSettingRepository;
            _saleRepository = saleRepository;
            _stockRepository = stockRepository;
            _saleCartDetailRepository = saleCartDetailRepository;
        }

        public IFinancialYearRepository FinancialYearRepository => _financialYearRepository;
        public IAccountSettingRepository AccountSettingRepository => _accountSettingRepository;
        public ISaleRepository SaleRepository => _saleRepository;
        public IStockRepository StockRepository => _stockRepository;
        public ISaleCartDetailRepository SaleCartDetailRepository => _saleCartDetailRepository;
    }
}
