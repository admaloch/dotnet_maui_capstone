using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace c971_project.Models
{


    public class Assessment : ObservableValidator
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentId { get; set; }
        public int CourseId { get; set; }    // FK to Course

        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(100, ErrorMessage = "Course name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = "Objective"; // Objective or Performance
        public string Status { get; set; } = "Planned"; // Default
        public DateTime DateAdded { get; set; } = DateTime.Now;
        // Existing date properties
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // NEW: Add these time properties
        public TimeSpan StartTime { get; set; } = new TimeSpan(9, 0, 0); // Default to 9:00 AM
        public TimeSpan EndTime { get; set; } = new TimeSpan(17, 0, 0);  // Default to 5:00 PM

        // NEW: Computed properties that combine date + time
        public DateTime StartDateTime => StartDate.Add(StartTime);
        public DateTime EndDateTime => EndDate.Add(EndTime);

        // Your existing notification properties
        public bool NotifyStartDate { get; set; } = true;
        public bool NotifyEndDate { get; set; } = true;
        public void Validate()
        {
            ValidateAllProperties();
        }
    }




}


