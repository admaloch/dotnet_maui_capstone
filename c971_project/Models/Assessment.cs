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
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public bool NotifyStartDate { get; set; } = false;
        public bool NotifyEndDate { get; set; } = false;
        public void Validate()
        {
            ValidateAllProperties();
        }
    }




}


