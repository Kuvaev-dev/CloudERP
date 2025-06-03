using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class TaskModel
    {
        public int TaskID { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [StringLength(100, ErrorMessageResourceName = "StringLengthMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Title { get; set; }

        [StringLength(1000, MinimumLength = 10, ErrorMessageResourceName = "StringLengthMinMaxValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public string? Description { get; set; }

        [Required(ErrorMessageResourceName = "RequiredValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DueDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ReminderDate { get; set; }
        public int? AssignedByUserID { get; set; }
        public int? AssignedToUserID { get; set; }
        public bool IsCompleted { get; set; }
        public int CompanyID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
    }
}