using System;
using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class TaskMV
    {
        public int TaskID { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReminderDate { get; set; }

        public bool IsCompleted { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public int UserID { get; set; }
    }
}