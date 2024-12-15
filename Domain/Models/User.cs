namespace Domain.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string ContactNo { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public int UserTypeID { get; set; }
        public bool IsActive { get; set; }
        public string UserTypeName { get; set; }
        public string BranchName { get; set; }
    }
}
