using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CloudERP.Models
{
    public class AccountSettingMV
    {
        public int AccountSettingID { get; set; }

        [Required(ErrorMessage = "Account Head is required.")]
        public int AccountHeadID { get; set; }

        [Required(ErrorMessage = "Account Control is required.")]
        public int AccountControlID { get; set; }

        [Required(ErrorMessage = "Account SubControl is required.")]
        public int AccountSubControlID { get; set; }

        [Required(ErrorMessage = "Account Activity is required.")]
        public int AccountActivityID { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public int BranchID { get; set; }

        public IEnumerable<SelectListItem> AccountHeadList { get; set; }
        public IEnumerable<SelectListItem> AccountControlList { get; set; }
        public IEnumerable<SelectListItem> AccountSubControlList { get; set; }
        public IEnumerable<SelectListItem> AccountActivityList { get; set; }
    }
}