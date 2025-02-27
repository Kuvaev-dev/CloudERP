namespace Domain.Models
{
    public class LoginResponse
    {
        public Domain.Models.User User { get; set; }
        public Domain.Models.Employee Employee { get; set; }
        public Domain.Models.Company Company { get; set; }
        public string Token { get; set; }
    }
}
