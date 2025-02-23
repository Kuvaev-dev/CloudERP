namespace Domain.Models.FinancialModels
{
    public class IncomeStatementModel
    {
        public string? Title { get; set; }
        public double NetIncome { get; set; }
        public List<IncomeStatementHead>? IncomeStatementHeads { get; set; }
    }

    public class IncomeStatementHead
    {
        public string? Title { get; set; }
        public double TotalAmount { get; set; }
        public AccountHeadTotal? AccountHead { get; set; }
    }
}
