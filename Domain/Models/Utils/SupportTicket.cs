using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class SupportTicket
    {
        public int TicketID { get; set; }

        [Required(ErrorMessage = "Subject is required.")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters.")]
        public string Subject { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(400, ErrorMessage = "Message cannot exceed 400 characters.")]
        public string Message { get; set; }

        public DateTime DateCreated { get; set; }
        public bool IsResolved { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public string AdminResponse { get; set; }
        public string RespondedBy { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}
