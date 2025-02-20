namespace CloudERP.Helpers
{
    public class SessionHelper
    {
        private readonly ISession _session;

        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor?.HttpContext?.Session ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        private int GetInt(string key)
        {
            var value = _session.GetString(key);
            if (int.TryParse(value, out int result))
            {
                return result;
            }
            throw new KeyNotFoundException($"{key} is not found in session.");
        }

        public int CompanyID => GetInt("CompanyID");
        public int BranchID => GetInt("BranchID");
        public int BrchID => GetInt("BrchID");
        public int UserID => GetInt("UserID");
        public int BranchTypeID => GetInt("BranchTypeID");

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

        public string InvoiceNo
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

        public string SaleInvoiceNo
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