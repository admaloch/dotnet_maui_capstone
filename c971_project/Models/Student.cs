using CommunityToolkit.Mvvm.ComponentModel;
using Firebase.Auth;
using System.ComponentModel.DataAnnotations;

namespace c971_project.Models
{
    public partial class Student : ObservableValidator
    {
        public Student()
        {
            id = Guid.NewGuid().ToString();
        }

        [ObservableProperty]
        private string id; // Firebase UID (primary key in Firestore)

        [ObservableProperty]
        [Required(ErrorMessage = "Student ID number is required.")]
        private string studentIdNumber = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Name is required.")]
        private string name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        private string email = string.Empty;

        [ObservableProperty]
        private string status = "Currently Enrolled";

        [ObservableProperty]
        [Required(ErrorMessage = "Major is required.")]
        private string major = string.Empty;

        [ObservableProperty]
        private DateTime dateAdded = DateTime.Now;

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}