using Domain.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CloudERP.Models
{
    public class AccountControlMV
    {
        private AccountControl _accountControl;

        public AccountControl AccountControl
        {
            get { return _accountControl ?? new AccountControl(); }
            set { _accountControl = value; }
        }

        public IEnumerable<SelectListItem> AccountHeadList { get; set; }
    }
}