namespace Domain.Models
{
    public class AccountSetting
    {
        public int AccountSettingID { get; set; }
        public int AccountHeadID { get; set; }
        public string AccountHeadName { get; set; }
        public int AccountControlID { get; set; }
        public string AccountControlName { get; set; }
        public int AccountSubControlID { get; set; }
        public string AccountSubControlName { get; set; }
        public int AccountActivityID { get; set; }
        public string AccountActivityName { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
    }
}
