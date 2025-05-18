using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupportTicket
    {
        public int TicketID { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(100, ErrorMessage = "Subject cannot exceed 100 characters.")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters.")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 1000 characters.")]
        public string Message { get; set; } = null!;

        public DateTime DateCreated { get; set; }

        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Admin response must be between 10 and 1000 characters, if provided.")]
        public string AdminResponse { get; set; } = null!;

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Responded by must be between 2 and 50 characters, if provided.")]
        public string RespondedBy { get; set; } = null!;

        public DateTime? ResponseDate { get; set; }

        public bool IsResolved { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }
    }
}