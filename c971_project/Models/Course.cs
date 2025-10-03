using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace c971_project.Models
{
    public class Course : ObservableValidator
    {
        [PrimaryKey, AutoIncrement]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(100, ErrorMessage = "Course name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course number is required.")]
        [MaxLength(20, ErrorMessage = "Course number cannot exceed 20 characters.")]
        public string CourseNum { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "Credit units must be between 1 and 10.")]
        public int CuNum { get; set; } = 3;
        public DateTime DateAdded { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Instructor is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid instructor.")]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Term is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid term.")]
        public int TermId { get; set; }
        // Existing date properties
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // NEW: Add these time propertieso
        public TimeSpan StartTime { get; set; } // Default to 9:00 AM
        public TimeSpan EndTime { get; set; }   // Default to 5:00 PM

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
