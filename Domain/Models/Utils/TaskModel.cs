using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class TaskModel
    {
        [Key]
        public int TaskID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due Date is required.")]
        public DateTime DueDate { get; set; }

        public DateTime? ReminderDate { get; set; }
        public bool IsCompleted { get; set; }
        public int? AssignedByUserID { get; set; }
        public int? AssignedToUserID { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}
