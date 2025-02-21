using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblAccountActivity
{
    public int AccountActivityID { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<tblAccountSetting> tblAccountSetting { get; set; } = new List<tblAccountSetting>();
}
