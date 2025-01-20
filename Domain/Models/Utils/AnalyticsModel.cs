namespace Domain.Models
{
    public class AnalyticsModel
    {
        // Employees
        public int TotalEmployees { get; set; }
        public int NewEmployeesThisMonth { get; set; }
        public int NewEmployeesThisYear { get; set; }

        // Stock
        public int TotalStockItems { get; set; }
        public int StockAvailable { get; set; }
        public int StockExpired { get; set; }

        // Support
        public int TotalSupportTickets { get; set; }
        public int ResolvedSupportTickets { get; set; }
        public int PendingSupportTickets { get; set; }
    }
}
