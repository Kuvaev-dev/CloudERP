namespace API.Models
{
    public class ResetPasswordRequest
    {
        public string ResetCode { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}