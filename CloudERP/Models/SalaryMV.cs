using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CloudERP.Models
{
    public class SalaryMV
    {
        public int EmployeeID { get; set; }
        [Required(ErrorMessage = "*Required!")]
        [DataType(DataType.Currency)]
        public double TransferAmount { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public string SalaryMonth { get; set; }
        [Required(ErrorMessage = "*Required!")]
        public string SalaryYear { get; set; }
    }
}