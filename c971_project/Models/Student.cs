using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace c971_project.Models
{
    public class Student: ObservableValidator
    {
        [PrimaryKey, AutoIncrement]
        public int StudentId { get; set; }
        [Required(ErrorMessage = "Student Id is required.")]
        public string StudentIdNumber { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")] public string Email { get; set; } = string.Empty;

        public string Status { get; set; } = "Currently Enrolled";

        [Required(ErrorMessage = "Major is required.")]
        public string Major { get; set; } = string.Empty;

        public DateTime DateAdded { get; set; } = DateTime.Now;


        public void Validate()
        {
            ValidateAllProperties();
        }
    }


}
