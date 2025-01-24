using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Services
{
    public interface IBalanceSheetService
    {
        Task<BalanceSheetModel> GetBalanceSheetAsync(int companyId, int branchId, int financialYearId, List<int> headIds);
    }

    public class BalanceSheetService : IBalanceSheetService
    {
        private readonly IBalanceSheetRepository _balanceSheetRepository;

        private const int ASSETS_HEAD_ID = 2;
        private const int LIABILITIES_HEAD_ID = 3;
        private const int OWNER_EQUITY_HEAD_ID = 4;
        private const int EXPENSES_HEAD_ID = 5;
        private const int REVENUE_HEAD_ID = 6;

        public BalanceSheetService(IBalanceSheetRepository balanceSheetRepository)
        {
            _balanceSheetRepository = balanceSheetRepository ?? throw new ArgumentNullException(nameof(IBalanceSheetRepository));
        }

        public async Task<BalanceSheetModel> GetBalanceSheetAsync(int companyId, int branchId, int financialYearId, List<int> headIds)
        {
            var BalanceSheet = new BalanceSheetModel();

            double TotalAssets = 0;
            double TotalLiabilities = 0;
            double TotalOwnerEquity = 0;
            double TotalReturnEarning = 0;

            // Return Earning
            double TotalExpenses = 0;
            double TotalRevenue = 0;

            var AllHeads = new List<AccountHeadTotal>();

            foreach (var HeadID in headIds)
            {
                AccountHeadTotal AccountHead = await _balanceSheetRepository.GetHeadAccountsWithTotal(companyId, branchId, financialYearId, HeadID);

                if (AccountHead != null && AccountHead.AccountHeadDetails != null)
                {
                    if (HeadID == ASSETS_HEAD_ID) // Total Assets
                    {
                        TotalAssets = await _balanceSheetRepository.GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == LIABILITIES_HEAD_ID) // Total Liabilities
                    {
                        TotalLiabilities = await _balanceSheetRepository.GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == OWNER_EQUITY_HEAD_ID) // Total Owner Equity
                    {
                        TotalOwnerEquity = await _balanceSheetRepository.GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == EXPENSES_HEAD_ID) // Total Expenses
                    {
                        TotalExpenses = AccountHead.TotalAmount;
                    }
                    else if (HeadID == REVENUE_HEAD_ID) // Total Revenue
                    {
                        TotalRevenue = AccountHead.TotalAmount;
                    }

                    AllHeads.Add(AccountHead);
                }
            }

            TotalReturnEarning = TotalRevenue - TotalExpenses;

            BalanceSheet.Title = Localization.Localization.TrialBalance;
            BalanceSheet.ReturnEarning = TotalReturnEarning;
            BalanceSheet.Total_Liabilities_OwnerEquity_ReturnEarning = TotalLiabilities + TotalOwnerEquity + TotalReturnEarning;
            BalanceSheet.TotalAssets = TotalAssets;
            BalanceSheet.AccountHeadTotals = AllHeads;

            return BalanceSheet;
        }
    }
}
