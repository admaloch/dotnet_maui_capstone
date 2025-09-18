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

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(4);

        public DateTime DateAdded { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Instructor is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid instructor.")]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Term is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid term.")]
        public int TermId { get; set; }

        public void Validate()
        {
            ValidateAllProperties();
        }
        public Course Clone()
        {
            return (Course)this.MemberwiseClone();
        }
    }
}
