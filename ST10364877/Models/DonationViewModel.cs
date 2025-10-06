using System.ComponentModel.DataAnnotations;

namespace ST10364877.Models
{
    public class DonationViewModel
    {
        [Required(ErrorMessage = "Contribution type is required")]
        [StringLength(100, ErrorMessage = "Contribution type cannot exceed 100 characters")]
        public string DonationType { get; set; }

        [Required(ErrorMessage = "Item description is required")]
        [StringLength(500, ErrorMessage = "Item description cannot exceed 500 characters")]
        public string ItemDescription { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public decimal Quantity { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
        public string Unit { get; set; }

        [Required(ErrorMessage = "Target area is required")]
        [StringLength(100, ErrorMessage = "Target area cannot exceed 100 characters")]
        public string TargetArea { get; set; }

        [StringLength(1000, ErrorMessage = "Special instructions cannot exceed 1000 characters")]
        public string SpecialInstructions { get; set; }
    }
}