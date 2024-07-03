namespace DatabaseAccess.Models
{
    public class DashboardModel
    {
        public double CurrentMonthRevenue { get; set; }
        public double CurrentMonthExpenses { get; set; }
        public double NetIncome { get; set; }
        public double CurrentMonthRecovery { get; set; }
        public double CashPlusBankAccountBalance { get; set; }
        public double TotalReceivable { get; set; }
        public double TotalPayable { get; set; }
        public double Capital { get; set; }

        // Today Sales / Purchases (Today)
        public double TotalSalesToday { get; set; }
        public double SalesPaymentPendingToday { get; set; }
        public double SalesPaymentPaidToday { get; set; }
        public double TotalPurchasesToday { get; set; }
        public double PurchasesPaymentPendingToday { get; set; }
        public double PurchasesPaymentPaidToday { get; set; }

        // Today Return Sales / Return Purchases (Today)
        public double TotalReturnSalesToday { get; set; }
        public double ReturnSalesPaymentPendingToday { get; set; }
        public double ReturnSalesPaymentPaidToday { get; set; }
        public double TotalReturnPurchasesToday { get; set; }
        public double ReturnPurchasesPaymentPendingToday { get; set; }
        public double ReturnPurchasesPaymentPaidToday { get; set; }

        // Sales / Purchases (Current Month)
        public double TotalSalesCurrentMonth { get; set; }
        public double SalesPaymentPendingCurrentMonth { get; set; }
        public double SalesPaymentPaidCurrentMonth { get; set; }
        public double TotalPurchasesCurrentMonth { get; set; }
        public double PurchasesPaymentPendingCurrentMonth { get; set; }
        public double PurchasesPaymentPaidCurrentMonth { get; set; }

        // Return Sales / Return Purchases (Current Month)
        public double TotalReturnSalesCurrentMonth { get; set; }
        public double ReturnSalesPaymentPendingCurrentMonth { get; set; }
        public double ReturnSalesPaymentPaidCurrentMonth { get; set; }
        public double TotalReturnPurchasesCurrentMonth { get; set; }
        public double ReturnPurchasesPaymentPendingCurrentMonth { get; set; }
        public double ReturnPurchasesPaymentPaidCurrentMonth { get; set; }
    }
}
