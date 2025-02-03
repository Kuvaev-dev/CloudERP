using Domain.Models.FinancialModels;
using Domain.RepositoryAccess;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private const int CAPITAL_HEAD_ID = 4;
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
            var tasks = new List<Task<AccountHeadTotal>>();

            foreach (var HeadID in headIds)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var accountHead = await _balanceSheetRepository.GetHeadAccountsWithTotal(companyId, branchId, financialYearId, HeadID);

                    if (accountHead?.AccountHeadDetails == null)
                        return null;

                    double? totalAmount = 0;

                    if (HeadID == ASSETS_HEAD_ID || HeadID == LIABILITIES_HEAD_ID || HeadID == CAPITAL_HEAD_ID)
                    {
                        totalAmount = await _balanceSheetRepository.GetAccountTotalAmountAsync(companyId, branchId, financialYearId, HeadID);
                    }

                    switch (HeadID)
                    {
                        case ASSETS_HEAD_ID:
                            TotalAssets = totalAmount ?? 0;
                            break;
                        case LIABILITIES_HEAD_ID:
                            TotalLiabilities = totalAmount ?? 0;
                            break;
                        case CAPITAL_HEAD_ID:
                            TotalOwnerEquity = totalAmount ?? 0;
                            break;
                        case EXPENSES_HEAD_ID:
                            TotalExpenses = accountHead.TotalAmount;
                            break;
                        case REVENUE_HEAD_ID:
                            TotalRevenue = accountHead.TotalAmount;
                            break;
                    }

                    return accountHead;
                }));
            }

            var results = await Task.WhenAll(tasks);

            AllHeads.AddRange(results.Where(a => a != null));

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
