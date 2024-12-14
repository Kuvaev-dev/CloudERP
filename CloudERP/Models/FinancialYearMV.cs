using System;
using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class FinancialYearMV
    {
        public int FinancialYearID { get; set; }

        [Required]
        [StringLength(50)]
        public string FinancialYearName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
    }
}