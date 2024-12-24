using System.Collections.Generic;

namespace Domain.Models.FinancialModels
{
    public class BalanceSheetModel
    {
        public string Title { get; set; }
        public double TotalAssets { get; set; }
        public double ReturnEarning { get; set; }
        public double Total_Liabilities_OwnerEquity_ReturnEarning { get; set; }
        public List<AccountHeadTotal> AccountHeadTotals { get; set; }
    }

    public class AccountHeadTotal
    {
        public string AccountHeadTitle { get; set; }
        public double TotalAmount { get; set; }
        public List<AccountHeadDetail> AccountHeadDetails { get; set; }
    }

    public class AccountHeadDetail
    {
        public string AccountSubTitle { get; set; }
        public double TotalAmount { get; set; }
        public string Status { get; set; }
    } 
}
