using System.Web.Mvc;

namespace CloudERP.Models
{
    public class AccountSettingMV
    {
        public SelectList AccountHeadList { get; set; }
        public SelectList AccountControlList { get; set; }
        public SelectList AccountSubControlList { get; set; }
        public SelectList AccountActivityList { get; set; }
    }
}