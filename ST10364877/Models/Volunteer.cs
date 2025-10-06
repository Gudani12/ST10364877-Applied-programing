using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST10364877.Models
{
    public class Volunteer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VolunteerId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Skills are required")]
        [StringLength(500, ErrorMessage = "Skills cannot exceed 500 characters")]
        public string Skills { get; set; }

        [Required(ErrorMessage = "Availability is required")]
        [StringLength(50, ErrorMessage = "Availability cannot exceed 50 characters")]
        public string Availability { get; set; }

        public bool HasTransportation { get; set; }

        [Required(ErrorMessage = "Preferred location is required")]
        [StringLength(100, ErrorMessage = "Preferred location cannot exceed 100 characters")]
        public string PreferredLocation { get; set; }

        [Required(ErrorMessage = "Emergency contact is required")]
        [StringLength(200, ErrorMessage = "Emergency contact cannot exceed 200 characters")]
        public string EmergencyContact { get; set; }

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string Status { get; set; } = "Active";

        // Navigation property - make it nullable
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}