using Domain.Models;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

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

        public IEnumerable<SelectListItem> BranchesList { get; set; }
        public HttpPostedFileBase LogoFile { get; set; }
    }
}