using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;


namespace c971_project.Models
{

    public partial class Note : ObservableValidator
    {
        public Note()
        {
            id = Guid.NewGuid().ToString();
        }

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string userId = string.Empty;

        [ObservableProperty]
        private string courseId = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        private string title = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Body is required.")]
        [MaxLength(5000, ErrorMessage = "Note body cannot exceed 5000 characters.")]
        private string body = string.Empty;

        [ObservableProperty]
        private DateTime dateAdded = DateTime.Now;

        [ObservableProperty]
        private DateTime lastUpdated = DateTime.Now;

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}


