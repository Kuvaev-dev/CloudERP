namespace CloudERP.Helpers
{
    public class SessionHelper
    {
        private readonly ISession _session;

        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor?.HttpContext?.Session ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public int CompanyID => _session.GetInt32("CompanyID") ?? throw new BadHttpRequestException("CompanyID not set in current session");
        public int BranchID => _session.GetInt32("BranchID") ?? throw new BadHttpRequestException("BranchID not set in current session");
        public int BrchID => _session.GetInt32("BrchID") ?? -1;
        public int UserID => _session.GetInt32("UserID") ?? throw new BadHttpRequestException("UserID not set in current session");
        public int BranchTypeID => _session.GetInt32("BranchTypeID") ?? throw new BadHttpRequestException("BranchTypeID not set in current session");

        public string? Token
        {
            get => _session.GetString("Token");
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _session.SetString("Token", value);
                }
                else
                {
                    _session.Remove("Token");
                }
            }
        }

        public int? CompanyEmployeeID
        {
            get
            {
                var value = _session.GetString("CEmployeeID");
                return int.TryParse(value, out int result) ? result : (int?)null;
            }
            set
            {
                if (value.HasValue)
                {
                    _session.SetString("CEmployeeID", value.Value.ToString());
                }
                else
                {
                    _session.Remove("CEmployeeID");
                }
            }
        }

        public string? InvoiceNo
        {
            get => _session.GetString("InvoiceNo");
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _session.SetString("InvoiceNo", value);
                }
                else
                {
                    _session.Remove("InvoiceNo");
                }
            }
        }

        public string? SaleInvoiceNo
        {
            get => _session.GetString("SaleInvoiceNo");
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _session.SetString("SaleInvoiceNo", value);
                }
                else
                {
                    _session.Remove("SaleInvoiceNo");
                }
            }
        }

        public bool IsAuthenticated => _session.GetString("CompanyID") != null && _session.GetString("BranchID") != null;
    }
}