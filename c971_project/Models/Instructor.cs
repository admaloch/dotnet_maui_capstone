using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;
using EmailAddressAttribute = System.ComponentModel.DataAnnotations.EmailAddressAttribute;

namespace c971_project.Models
{
    public class Instructor : ObservableValidator
    {
        [PrimaryKey, AutoIncrement]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Instructor name is required.")]
        [MaxLength(100, ErrorMessage = "Instructor name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
            ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [MaxLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}