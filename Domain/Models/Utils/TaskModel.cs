using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class TaskModel
    {
        public int TaskID { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string? Title { get; set; }

        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters, if provided.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReminderDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Assigned by user ID must be a positive integer, if provided.")]
        public int? AssignedByUserID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Assigned to user ID must be a positive integer, if provided.")]
        public int? AssignedToUserID { get; set; }

        public bool IsCompleted { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Company ID must be a positive integer.")]
        public int CompanyID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Branch ID must be a positive integer.")]
        public int BranchID { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "User ID must be a positive integer.")]
        public int UserID { get; set; }
    }
}
