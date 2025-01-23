using Domain.Models;
using System.Web;

namespace CloudERP.Models
{
    public class EmployeeMV
    {
        private Employee _employee;

        public Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }

        public HttpPostedFileBase LogoFile { get; set; }
    }
}