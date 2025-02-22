using System;

namespace Domain.Models
{
    public class SupportTicket
    {
        public int TicketID { get; set; }
        public string Subject { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; }
        public string? AdminResponse { get; set; }
        public string? RespondedBy { get; set; }
        public DateTime? ResponseDate { get; set; }
        public bool IsResolved { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}
