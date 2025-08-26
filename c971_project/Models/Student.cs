using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;
using MinLengthAttribute = System.ComponentModel.DataAnnotations.MinLengthAttribute;
namespace c971_project.Models
{
    public partial class Student : ObservableValidator
    {
        // Backing fields initialized to avoid nulls
        private string _studentId = string.Empty;
        private string _name = string.Empty;
        private string _email = string.Empty;
        private string _status = "Currently Enrolled";
        private string _major = string.Empty;

        [PrimaryKey]
        public string StudentId
        {
            get => _studentId;
            set => SetProperty(ref _studentId, value, true); // true enables validation
        }

        [Required(ErrorMessage = "Full name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, true);
        }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value, true);
        }

        [Required(ErrorMessage = "Status is required")]
        public string Status
        {
            get => _status;
            set => SetProperty(ref _status, value, true);
        }

        [MaxLength(50, ErrorMessage = "Major cannot exceed 50 characters")]
        public string Major
        {
            get => _major;
            set => SetProperty(ref _major, value, true);
        }

        // Helper to validate all properties
        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }

        // Optional: safe helper to get error for a property
        public string? GetFirstError(string propertyName)
        {
            var errors = GetErrors(propertyName);
            if (errors != null)
            {
                foreach (var err in errors)
                    return err.ErrorMessage;
            }
            return null;
        }
    }
}
