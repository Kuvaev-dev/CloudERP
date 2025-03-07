using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using Domain.ServiceAccess;

namespace Services.Implementations
{
    public class BalanceSheetService : IBalanceSheetService
    {
        private readonly IBalanceSheetRepository _balanceSheetRepository;

        private const int ASSETS_HEAD_ID = 2;
        private const int LIABILITIES_HEAD_ID = 3;
        private const int CAPITAL_HEAD_ID = 4;
        private const int EXPENSES_HEAD_ID = 5;
        private const int REVENUE_HEAD_ID = 6;

        public BalanceSheetService(IBalanceSheetRepository balanceSheetRepository)
        {
            _balanceSheetRepository = balanceSheetRepository ?? throw new ArgumentNullException(nameof(balanceSheetRepository));
        }

        public async Task<BalanceSheetModel> GetBalanceSheetAsync(int companyId, int branchId, int financialYearId, List<int> headIds)
        {
            var balanceSheet = new BalanceSheetModel();

            double totalAssets = 0;
            double totalLiabilities = 0;
            double totalOwnerEquity = 0;
            double totalReturnEarning = 0;

            // Return Earning
            double totalExpenses = 0;
            double totalRevenue = 0;

            var allHeads = new List<AccountHeadTotal>();

            foreach (var HeadID in headIds)
            {
                var accountHead = await _balanceSheetRepository.GetHeadAccountsWithTotal(companyId, branchId, financialYearId, HeadID);

                if (accountHead?.AccountHeadDetails == null)
                    continue;

                double? totalAmount = 0;

                if (HeadID == ASSETS_HEAD_ID || HeadID == LIABILITIES_HEAD_ID || HeadID == CAPITAL_HEAD_ID)
                {
                    totalAmount = await _balanceSheetRepository.GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                }

                switch (HeadID)
                {
                    case ASSETS_HEAD_ID:
                        totalAssets = totalAmount ?? 0;
                        break;
                    case LIABILITIES_HEAD_ID:
                        totalLiabilities = totalAmount ?? 0;
                        break;
                    case CAPITAL_HEAD_ID:
                        totalOwnerEquity = totalAmount ?? 0;
                        break;
                    case EXPENSES_HEAD_ID:
                        totalExpenses = accountHead.TotalAmount;
                        break;
                    case REVENUE_HEAD_ID:
                        totalRevenue = accountHead.TotalAmount;
                        break;
                }

                allHeads.Add(accountHead);
            }

            totalReturnEarning = totalRevenue - totalExpenses;

            balanceSheet.Title = Localization.Services.Localization.BalanceSheet;
            balanceSheet.ReturnEarning = totalReturnEarning;
            balanceSheet.Total_Liabilities_OwnerEquity_ReturnEarning = totalLiabilities + totalOwnerEquity + totalReturnEarning;
            balanceSheet.TotalAssets = totalAssets;
            balanceSheet.AccountHeadTotals = allHeads;

            return balanceSheet;
        }
    }
}
