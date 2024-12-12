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

        public bool IsAuthenticated => _session["CompanyID"] != null && _session["BranchID"] != null;
    }
}