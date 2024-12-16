using System.ComponentModel.DataAnnotations;
using System.Web;

namespace CloudERP.Models
{
    public class CompanyMV
    {
        public int CompanyID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Logo { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public HttpPostedFileBase LogoFile { get; set; }
    }
}