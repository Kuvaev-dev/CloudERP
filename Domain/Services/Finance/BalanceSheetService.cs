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
                    if (HeadID == 3) // Total Assets
                    {
                        TotalAssets = await _balanceSheetRepository.GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == 4) // Total Liabilities
                    {
                        TotalLiabilities = await _balanceSheetRepository.GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == 7) // Total Owner Equity
                    {
                        TotalOwnerEquity = await _balanceSheetRepository.GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }
                    else if (HeadID == 5) // Total Expenses
                    {
                        TotalExpenses = AccountHead.TotalAmount;
                    }
                    else if (HeadID == 6) // Total Revenue
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
