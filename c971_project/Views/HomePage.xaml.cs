using c971_project.Services;
using c971_project.Models;
using System.ComponentModel;

namespace c971_project.Views
{
    public partial class HomePage : ContentPage, INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private Student _currentStudent;

        public Student CurrentStudent
        {
            get => _currentStudent;
            set
            {
                _currentStudent = value;
                OnPropertyChanged(nameof(CurrentStudent));
            }
        }

        public HomePage(DatabaseService databaseService)
        {
            InitializeComponent();
            _databaseService = databaseService;

            // Set binding context
            BindingContext = this;

            LoadStudentData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadStudentData(); // Refresh data when page appears
        }

        private void LoadStudentData()
        {
            var students = _databaseService.GetStudents();
            if (students.Any())
            {
                CurrentStudent = students.First();
            }
            else
            {
                // Fallback if no student data
                CurrentStudent = new Student
                {
                    StudentId = "N/A",
                    Name = "No Student Data",
                    Email = "N/A",
                    Status = "Not Enrolled",
                    Major = "N/A"
                };
            }
        }

        // INotifyPropertyChanged implementation
        public new event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}