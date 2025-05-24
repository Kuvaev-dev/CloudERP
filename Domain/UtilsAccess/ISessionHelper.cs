namespace Domain.UtilsAccess
{
    public interface ISessionHelper
    {
        int CompanyID { get; }
        int BranchID { get; }
        int BrchID { get; }
        int UserID { get; }
        int BranchTypeID { get; }
        string? Token { get; set; }
        int? CompanyEmployeeID { get; set; }
        string? InvoiceNo { get; set; }
        string? SaleInvoiceNo { get; set; }
        bool IsAuthenticated { get; }
    }
}
