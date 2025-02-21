using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblTask
{
    public int TaskID { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime DueDate { get; set; }

    public DateTime? ReminderDate { get; set; }

    public bool IsCompleted { get; set; }

    public int CompanyID { get; set; }

    public int BranchID { get; set; }

    public int UserID { get; set; }

    public int? AssignedByUserID { get; set; }

    public int? AssignedToUserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;
}
