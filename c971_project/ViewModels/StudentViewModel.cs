using c971_project.Models;
using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace c971_project.ViewModels
{
    public partial class StudentViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Student _student = new();

        // Add this property for query parameter binding
        [ObservableProperty]
        private string _studentId;

        public ObservableCollection<string> StatusOptions { get; } = new()
        {
            "Currently Enrolled",
            "Graduated",
            "Withdrawn",
            "Academic Leave",
            "Not Enrolled"
        };

        public StudentViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Add this partial method to handle StudentId changes
        partial void OnStudentIdChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                // Load the student when StudentId is set via query parameter
                Task.Run(async () => await LoadStudentAsync(value));
            }
        }

        public async Task LoadStudentAsync(string studentId = null)
        {
            if (!string.IsNullOrEmpty(studentId))
            {
                var existingStudent = await _databaseService.GetStudentAsync(studentId);
                if (existingStudent != null)
                {
                    Student = existingStudent;
                }
            }
        }

        [RelayCommand]
        private async Task SaveStudentAsync()
        {
            if (Student.Validate())
            {
                try
                {
                    if (string.IsNullOrEmpty(Student.StudentId) || Student.StudentId == "N/A")
                    {
                        Student.StudentId = GenerateNewStudentId();
                        await _databaseService.InsertStudentAsync(Student);
                        await Shell.Current.DisplayAlert("Success", "Student added successfully.", "OK");
                    }
                    else
                    {
                        await _databaseService.UpdateStudentAsync(Student);
                        await Shell.Current.DisplayAlert("Success", "Student information updated.", "OK");
                    }

                    await Shell.Current.GoToAsync("..");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", $"Failed to save student: {ex.Message}", "OK");
                }
            }
            else
            {
                var errors = string.Join("\n", Student.GetErrors()
                    .Select(e => e.ErrorMessage));
                await Shell.Current.DisplayAlert("Validation Error", errors, "OK");
            }
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private string GenerateNewStudentId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}