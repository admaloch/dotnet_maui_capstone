using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using EmailAddressAttribute = System.ComponentModel.DataAnnotations.EmailAddressAttribute;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace c971_project.Models
{
    public partial class Instructor : ObservableValidator
    {
        public Instructor()
        {
            id = Guid.NewGuid().ToString();
        }

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        [Required(ErrorMessage = "Instructor name is required.")]
        [MaxLength(100, ErrorMessage = "Instructor name cannot exceed 100 characters.")]
        private string name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Phone number is required.")]
        [MaxLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
            ErrorMessage = "Please enter a valid phone number.")]
        private string phone = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Email address is required.")]
        [MaxLength(100, ErrorMessage = "Email address cannot exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        private string email = string.Empty;

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}