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
        [Required(ErrorMessage = "Student ID is required")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Student ID must contain only numbers")]
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

        // helper to get error messages
        public String GetStudentErrors(Student student)
        {
            // Validate and use the returned bool
            var valid = student.Validate(); // your helper calls ValidateAllProperties()
            var errorMessage = "";

            if (!valid)
            {
                // Collect all error messages (explicit property list)
                var propertiesToCheck = new[] { nameof(student.StudentId), nameof(student.Name), nameof(student.Email), nameof(student.Status), nameof(student.Major) };
                var allErrors = new List<string>();

                foreach (var prop in propertiesToCheck)
                {
                    var errors = student.GetErrors(prop);
                    if (errors != null)
                    {
                        foreach (var err in errors)
                            allErrors.Add(err.ErrorMessage);
                    }
                }

                errorMessage = allErrors.Count > 0
                    ? string.Join(Environment.NewLine + Environment.NewLine, allErrors)
                    : "Please fix the validation errors.";

            }
            return errorMessage; // <-- important: stop here, do not save
        }
        public Student Clone()
        {
            return new Student
            {
                StudentId = this.StudentId,
                Name = this.Name,
                Email = this.Email,
                Status = this.Status,
                Major = this.Major
            };
        }
    }
}
