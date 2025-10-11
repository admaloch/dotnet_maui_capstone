using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;
using System.ComponentModel.DataAnnotations;
using MaxLengthAttribute = System.ComponentModel.DataAnnotations.MaxLengthAttribute;

namespace c971_project.Models
{


    public partial class Assessment : ObservableValidator
    {
        public Assessment()
        {
            id = Guid.NewGuid().ToString();
        }

        [ObservableProperty]
        private string id;

        [ObservableProperty]
        private string courseId = string.Empty;

        [ObservableProperty]
        [Required(ErrorMessage = "Assessment name is required.")]
        [MaxLength(100, ErrorMessage = "Assessment name cannot exceed 100 characters.")]
        private string name = string.Empty;

        [ObservableProperty]
        private string type = "Objective"; // Objective or Performance

        [ObservableProperty]
        private string status = "Planned"; // Default

        [ObservableProperty]
        private DateTime dateAdded = DateTime.Now;

        [ObservableProperty]
        private DateTime startDate;

        [ObservableProperty]
        private DateTime endDate;

        [ObservableProperty]
        private TimeSpan startTime = new TimeSpan(9, 0, 0); // Default to 9:00 AM

        [ObservableProperty]
        private TimeSpan endTime = new TimeSpan(17, 0, 0);  // Default to 5:00 PM

        [ObservableProperty]
        private bool notifyStartDate = true;

        [ObservableProperty]
        private bool notifyEndDate = true;

        // Computed properties (no ObservableProperty needed)
        public DateTime StartDateTime => StartDate.Add(StartTime);
        public DateTime EndDateTime => EndDate.Add(EndTime);

        public void Validate()
        {
            ValidateAllProperties();
        }
    }




}


