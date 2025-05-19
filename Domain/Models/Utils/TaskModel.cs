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
        [DataType(DataType.Date, ErrorMessageResourceName = "DateValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public DateTime DueDate { get; set; }

        [DataType(DataType.Date, ErrorMessageResourceName = "DateValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public DateTime? ReminderDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int? AssignedByUserID { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int? AssignedToUserID { get; set; }

        public bool IsCompleted { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int CompanyID { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int BranchID { get; set; }

        [Range(1, int.MaxValue, ErrorMessageResourceName = "RangeValidation", ErrorMessageResourceType = typeof(Localization.Domain.Localization))]
        public int UserID { get; set; }
    }
}