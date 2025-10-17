using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace c971_project.Models
{
    public partial class Course : ObservableValidator
    {
        public Course()
        {
            id = Guid.NewGuid().ToString();
        }

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string userId = string.Empty;

        [ObservableProperty]
        private string termId = string.Empty;

        [ObservableProperty]
        private string instructorId = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(100, ErrorMessage = "Course name cannot exceed 100 characters.")]
        private string name = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Course number is required.")]
        [MaxLength(20, ErrorMessage = "Course number cannot exceed 20 characters.")]
        private string courseNum = string.Empty;

        [ObservableProperty]
        [Range(1, 10, ErrorMessage = "Credit units must be between 1 and 10.")]
        private int cuNum = 3;

        [ObservableProperty]
        private DateTime dateAdded = DateTime.Now;

        [ObservableProperty]
        private DateTime startDate;

        [ObservableProperty]
        private DateTime endDate;

        [ObservableProperty]
        private TimeSpan startTime = new TimeSpan(9, 0, 0);

        [ObservableProperty]
        private TimeSpan endTime = new TimeSpan(17, 0, 0);

        [ObservableProperty]
        private bool notifyStartDate = true;

        [ObservableProperty]
        private bool notifyEndDate = true;

        public DateTime StartDateTime => StartDate.Add(StartTime);
        public DateTime EndDateTime => EndDate.Add(EndTime);

        public void Validate()
        {
            ValidateAllProperties();
        }
    }
}
