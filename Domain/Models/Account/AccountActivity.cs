namespace Domain.Models
{
    public class AccountActivity
    {
        public int AccountActivityID { get; set; }
        public string Name { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
    }
}
