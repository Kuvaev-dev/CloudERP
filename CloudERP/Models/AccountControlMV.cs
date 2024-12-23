using Domain.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CloudERP.Models
{
    public class AccountControlMV
    {
        public int AccountControlID { get; set; }

        [Required(ErrorMessage = "Account name is required.")]
        [StringLength(100, ErrorMessage = "Account name must not exceed 100 characters.")]
        public string AccountControlName { get; set; }

        [Required(ErrorMessage = "Head account ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Head account ID must be greater than zero.")]
        public int AccountHeadID { get; set; }

        [Required(ErrorMessage = "Branch ID is required.")]
        public int BranchID { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public int CompanyID { get; set; }

        [Required(ErrorMessage = "User  ID is required.")]
        public int UserID { get; set; }




        private AccountHead myVar;

        public AccountHead MyProperty
        {
            get { return myVar ?? new AccountHead(); }
        }

        public IEnumerable<SelectListItem> AccountHeadList { get; set; }
    }
}