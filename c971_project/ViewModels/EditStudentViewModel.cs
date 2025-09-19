using c971_project.Messages;
using c971_project.Models;
using c971_project.Services;
using c971_project.Helpers;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;


namespace c971_project.ViewModels
{
    [QueryProperty(nameof(StudentId), "StudentId")]
    public partial class EditStudentViewModel : BaseViewModel
    {

        [ObservableProperty]
        private string studentId;

        [ObservableProperty]
        private Student _student;

        private readonly DatabaseService _databaseService;

        public List<string> StatusOptions { get; } = new()
        {
            "Not Enrolled Yet",
            "Currently Enrolled",
            "Paused",
            "Graduated"
        };

        public EditStudentViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // This runs when student id changes
        partial void OnStudentIdChanged(string value)
        {
            Debug.WriteLine($"CourseId set via query: {value}");

            if (Student == null)
            {
                Student = new Student
                {
                    StudentId = value,
                    Name = string.Empty,
                    Email = string.Empty,
                    Status = "Not Enrolled Yet",
                    Major = string.Empty,
                    DateAdded = DateTime.Today,
                };
            }
            else
            {
                // If student id exists already, just update the id
                Student.StudentId = value;
            }
        }


        [RelayCommand]
        private async Task SaveStudentAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var isTaskValid = await ValidateStudentAsync();

                if (!isTaskValid)
                    return;

                // Save to database
                await _databaseService.SaveStudentAsync(Student);

                // Optional: notify other viewmodels
                WeakReferenceMessenger.Default.Send(new StudentUpdatedMessage());

                await Shell.Current.DisplayAlert("Success", "Student saved successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving student: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to save student. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<bool> ValidateStudentAsync()
        {
            Student.Validate();

            if (Student.HasErrors)
            {
                var errorMessage = ValidationHelper.GetErrors(
                    Student,
                    nameof(Student.StudentId),
                    nameof(Student.Name),
                    nameof(Student.Email),
                    nameof(Student.Major)

                );

                await Shell.Current.DisplayAlert("Validation Errors", errorMessage, "OK");
                return false;
            }
            return true;
        }


    }
}
