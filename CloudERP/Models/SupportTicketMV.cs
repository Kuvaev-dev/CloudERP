using System;
using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class SupportTicketMV
    {
        public int TicketID { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public DateTime DateCreated { get; set; }
        public bool IsResolved { get; set; }
    }
}