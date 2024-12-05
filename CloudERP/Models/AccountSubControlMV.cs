using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CloudERP.Models
{
    public class AccountSubControlMV
    {
        public int AccountSubControlID { get; set; }

        [Required(ErrorMessage = "Account sub-control name is required.")]
        [StringLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters and spaces are allowed.")]
        public string AccountSubControlName { get; set; }

        [Required(ErrorMessage = "Account control is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid account control.")]
        public int AccountControlID { get; set; }

        public int AccountHeadID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserID { get; set; }

        public IEnumerable<SelectListItem> AccountControlList { get; set; }
    }
}