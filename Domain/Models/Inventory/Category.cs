using System.ComponentModel.DataAnnotations;

namespace Domain.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, ErrorMessage = "Category Name cannot exceed 100 characters.")]
        public string CategoryName { get; set; }
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
    }
}
