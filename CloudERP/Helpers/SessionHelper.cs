using System.Collections.Generic;
using System.Web;

namespace CloudERP.Helpers
{
    public class SessionHelper
    {
        private readonly HttpSessionStateBase _session;

        public SessionHelper(HttpSessionStateBase session)
        {
            _session = session;
        }

        public int CompanyID
        {
            get
            {
                if (int.TryParse(_session["CompanyID"]?.ToString(), out int value))
                {
                    return value;
                }
                throw new KeyNotFoundException("CompanyID is not found in session.");
            }
        }

        public int BranchID
        {
            get
            {
                if (int.TryParse(_session["BranchID"]?.ToString(), out int value))
                {
                    return value;
                }
                throw new KeyNotFoundException("BranchID is not found in session.");
            }
        }

        public int BrchID
        {
            get
            {
                if (int.TryParse(_session["BrchID"]?.ToString(), out int value))
                {
                    return value;
                }
                throw new KeyNotFoundException("BrchID is not found in session.");
            }
        }

        public int UserID
        {
            get
            {
                if (int.TryParse(_session["UserID"]?.ToString(), out int value))
                {
                    return value;
                }
                throw new KeyNotFoundException("UserID is not found in session.");
            }
        }

        public int BranchTypeID
        {
            get
            {
                if (int.TryParse(_session["BranchTypeID"]?.ToString(), out int value))
                {
                    return value;
                }
                throw new KeyNotFoundException("BranchTypeID is not found in session.");
            }
        }

        public int? CompanyEmployeeID
        {
            get
            {
                if (int.TryParse(_session["CEmployeeID"]?.ToString(), out int value))
                {
                    return value;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    _session["CEmployeeID"] = value.Value;
                }
                else
                {
                    _session.Remove("CEmployeeID");
                }
            }
        }

        private string _invoiceNo;
        public string InvoiceNo
        {
            get
            {
                if (!string.IsNullOrEmpty(_session["InvoiceNo"].ToString()))
                {
                    _invoiceNo = _session["InvoiceNo"].ToString();
                    return _invoiceNo;
                }
                return null;
            }
            set
            {
                if (!string.IsNullOrEmpty(_session["InvoiceNo"].ToString()))
                {
                    _invoiceNo = _session["InvoiceNo"].ToString();
                }
            }
        }

        public bool IsAuthenticated => _session["CompanyID"] != null && _session["BranchID"] != null;
    }
}