using c971_project.Messages;
using c971_project.Models;
using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Diagnostics;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(Student), "Student")]
    public partial class StudentViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Student student;

        private readonly DatabaseService _databaseService;

        public List<string> StatusOptions { get; } = new()
        {
            "Not Enrolled Yet",
            "Currently Enrolled",
            "Paused",
            "Graduated"
        };

        public StudentViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync(".."); // navigate back
        }

        [RelayCommand]
        private async Task SaveStudentAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                //validate and return error string
                String ErrorMessage = Student.GetStudentErrors(Student);

                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", ErrorMessage, "OK");
                    return;
                }

                // save if no errors
                await _databaseService.SaveStudentAsync(Student);

                // notify other viewmodels/pages if you use messaging
                WeakReferenceMessenger.Default.Send(new StudentUpdatedMessage());

                await Shell.Current.DisplayAlert("Success", "Student saved successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving student data: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to save student data. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


    }
}
