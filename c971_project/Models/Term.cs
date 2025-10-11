using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.ComponentModel.DataAnnotations;
using SQLite;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;
using MinLengthAttribute = System.ComponentModel.DataAnnotations.MinLengthAttribute;

namespace c971_project.Models
{
    public partial class Term : ObservableValidator
    {
        public Term()
        {
            id = Guid.NewGuid().ToString();
        }

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string userId = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Term name is required")]
        [MinLength(2, ErrorMessage = "Term name must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Term name cannot exceed 50 characters")]
        private string name = string.Empty;

        [ObservableProperty]
        private DateTime startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime endDate = DateTime.Today.AddMonths(4);

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}
