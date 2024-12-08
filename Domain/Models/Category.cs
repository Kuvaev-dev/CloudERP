namespace Domain.Models
{
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
