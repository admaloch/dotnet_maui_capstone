using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace c971_project.Models
{
    public class Student: ObservableValidator
    {
        [PrimaryKey]
        [Required(ErrorMessage = "Student Id is required.")]
        public string StudentId { get; set; } = string.Empty; // Cha ged to string! 

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; } = string.Empty;

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
