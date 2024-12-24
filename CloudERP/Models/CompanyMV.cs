using Domain.Models;
using System.Web;

namespace CloudERP.Models
{
    public class CompanyMV
    {
        private Company _company;

        public Company Company
        {
            get { return _company ?? new Company(); }
            set { _company = value; }
        }

        public HttpPostedFileBase LogoFile { get; set; }
    }
}