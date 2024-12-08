namespace Domain.Models
{
    public class AccountSubControl
    {
        public int AccountSubControlID { get; set; }
        public string AccountSubControlName { get; set; }
        public int AccountControlID { get; set; }
        public string AccountControlName { get; set; }
        public int AccountHeadID { get; set; }
        public string AccountHeadName { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public string FullName { get; set; }
    }
}
