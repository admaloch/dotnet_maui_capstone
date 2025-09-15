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
        [PrimaryKey, AutoIncrement]
        public int TermId { get; set; }

        [Required(ErrorMessage = "Term name is required")]
        [MinLength(2, ErrorMessage = "Term name must be at least 2 characters")]
        [MaxLength(50, ErrorMessage = "Term name cannot exceed 50 characters")]
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value, true); // true enables validation
        }
        private string _name = string.Empty;

        public int TermNum
        {
            get => _termNum;
            set => SetProperty(ref _termNum, value);
        }
        private int _termNum;

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value, true);
        }
        private DateTime _startDate = DateTime.Today;

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }
        private DateTime _endDate = DateTime.Today.AddMonths(4); // default length

        // Helper to validate all properties
        public void Validate()
        {
            ValidateAllProperties();
        }


    }
}
