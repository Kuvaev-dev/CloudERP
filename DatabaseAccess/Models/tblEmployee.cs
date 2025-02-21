using System;
using System.Collections.Generic;

namespace DatabaseAccess.Models;

public partial class tblEmployee
{
    public int EmployeeID { get; set; }

    public string Name { get; set; } = null!;

    public string ContactNo { get; set; } = null!;

    public string? Photo { get; set; }

    public string Email { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string TIN { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public string Description { get; set; } = null!;

    public double MonthlySalary { get; set; }

    public DateTime? RegistrationDate { get; set; }

    public bool? IsFirstLogin { get; set; }

    public int BranchID { get; set; }

    public int CompanyID { get; set; }

    public int? UserID { get; set; }

    public virtual tblBranch Branch { get; set; } = null!;

    public virtual tblCompany Company { get; set; } = null!;

    public virtual ICollection<tblPayroll> tblPayroll { get; set; } = new List<tblPayroll>();
}
