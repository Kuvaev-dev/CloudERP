using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblSupportTicket
{
    public int TicketID { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public bool IsResolved { get; set; }

    public int BranchID { get; set; }

    public int CompanyID { get; set; }

    public int UserID { get; set; }

    public string? AdminResponse { get; set; }

    public string? RespondedBy { get; set; }

    public DateTime? ResponseDate { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual tblUser User { get; set; } = null!;
}
