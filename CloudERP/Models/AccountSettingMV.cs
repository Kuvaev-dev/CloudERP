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
        public string AccountHeadName { get; set; }

        [Required(ErrorMessage = "Account Control is required.")]
        public int AccountControlID { get; set; }
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "Account SubControl is required.")]
        public int AccountSubControlID { get; set; }
        public string AccountSubControlName { get; set; }

        [Required(ErrorMessage = "Account Activity is required.")]
        public int AccountActivityID { get; set; }
        public string AccountActivityName { get; set; }

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