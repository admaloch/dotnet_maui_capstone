using SQLite;
using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;
using MinLengthAttribute = System.ComponentModel.DataAnnotations.MinLengthAttribute;

namespace c971_project.Models
{
    public partial class Student : ObservableValidator
    {
        [PrimaryKey]
        public string StudentId
        {
            get => _studentId;
            set => SetProperty(ref _studentId, value, true); // true enables validation
        }

        [Required(ErrorMessage = "Student ID is required")]
        [MinLength(3)]
        [MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z0-9]+$")]
        private string _studentId = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Full name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        private string _name = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        private string _email = string.Empty;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Status is required")]
        private string _status = "Currently Enrolled";

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [MaxLength(50, ErrorMessage = "Major cannot exceed 50 characters")]
        private string _major = string.Empty;
        public bool Validate()
        {
            ValidateAllProperties();
            return !HasErrors;
        }
    }

}