using c971_project.Models;
using c971_project.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace c971_project.Views
{
    public partial class EditStudentPage : ContentPage, INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;

        private Student _student;

        public Student Student
        {
            get => _student;
            set
            {
                _student = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> StatusOptions { get; } = new ObservableCollection<string>
        {
            "Currently Enrolled",
            "Graduated",
            "Withdrawn",
            "Academic Leave",
            "Not Enrolled"
        };

        public EditStudentPage(DatabaseService databaseService, Student student = null)
        {
            InitializeComponent();
            _databaseService = databaseService;

            // If student is provided, edit mode; otherwise, create new
            Student = student ?? new Student();

            BindingContext = this;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                // Basic validation
                if (string.IsNullOrWhiteSpace(Student.Name) ||
                    string.IsNullOrWhiteSpace(Student.StudentId))
                {
                    await DisplayAlert("Error", "Name and Student ID are required.", "OK");
                    return;
                }

                // Check if this is a new student or existing one
                if (Student.StudentId == "N/A")
                {
                    // Generate a new student ID for new students
                    Student.StudentId = GenerateNewStudentId();
                    await _databaseService.InsertStudentAsync(Student); // Use new async method
                    await DisplayAlert("Success", "Student added successfully.", "OK");
                }
                else
                {
                    await _databaseService.UpdateStudentAsync(Student); // Use new async method
                    await DisplayAlert("Success", "Student information updated.", "OK");
                }

                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to save student: {ex.Message}", "OK");
            }
        }

        private string GenerateNewStudentId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss"); // Simple timestamp ID
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        // INotifyPropertyChanged implementation
        public new event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null!)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}