using System.ComponentModel.DataAnnotations;

namespace ST10364877.Models
{
  
        public class IncidentReportViewModel
        {
            [Required(ErrorMessage = "Title is required")]
            [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
            public string Title { get; set; }

            [Required(ErrorMessage = "Description is required")]
            [StringLength(4000, ErrorMessage = "Description cannot exceed 4000 characters")]
            public string Description { get; set; }

            [Required(ErrorMessage = "Location is required")]
            [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
            public string Location { get; set; }

            [Required(ErrorMessage = "Incident date is required")]
            [DataType(DataType.DateTime)]
            public DateTime IncidentDate { get; set; }

            [StringLength(50)]
            public string DisasterType { get; set; }

            [StringLength(500, ErrorMessage = "Affected areas cannot exceed 500 characters")]
            public string AffectedAreas { get; set; }

            [StringLength(20)]
            public string UrgencyLevel { get; set; } = "Medium";
        }
    }

