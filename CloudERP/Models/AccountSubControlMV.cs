using Domain.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CloudERP.Models
{
    public class AccountSubControlMV
    {
        private AccountSubControl _accountSubControl;
        public AccountSubControl AccountSubControl
        {
            get { return _accountSubControl; }
            set { _accountSubControl = value; }
        }

        public IEnumerable<SelectListItem> AccountControlList { get; set; }
    }
}