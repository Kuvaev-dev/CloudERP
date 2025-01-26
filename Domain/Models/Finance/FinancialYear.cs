using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class FinancialYear
    {
        [Key]
        public int FinancialYearID { get; set; }

        [Required(ErrorMessage = "Financial Year Name is required.")]
        [StringLength(100, ErrorMessage = "Financial Year Name cannot exceed 100 characters.")]
        public string FinancialYearName { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid Start Date format.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Invalid End Date format.")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "IsActive status is required.")]
        public bool IsActive { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}