namespace Domain.Models
{
    public class Branch
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string BranchContact { get; set; }
        public string BranchAddress { get; set; }
        public int CompanyID { get; set; }
        public int? ParentBranchID { get; set; }
        public int BranchTypeID { get; set; }
        public string BranchTypeName { get; set; }
    }
}
