namespace Domain.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Customername { get; set; }
        public string CustomerContact { get; set; }
        public string CustomerAddress { get; set; }
        public string Description { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public string UserName { get; set; }
    }
}
