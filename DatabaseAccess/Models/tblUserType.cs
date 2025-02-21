using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblUserType
{
    public int UserTypeID { get; set; }

    public string UserType { get; set; } = null!;

    public virtual ICollection<tblUser> tblUser { get; set; } = new List<tblUser>();
}
