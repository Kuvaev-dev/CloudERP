using System.ComponentModel.DataAnnotations;

namespace CloudERP.Models
{
    public class CategoryMV
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Category Name is required.")]
        [StringLength(100, ErrorMessage = "Category Name must not exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Only letters and spaces are allowed.")]
        public string CategoryName { get; set; }

        public int UserID { get; set; }
    }
}