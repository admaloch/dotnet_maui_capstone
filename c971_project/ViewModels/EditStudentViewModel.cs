using c971_project.Messages;
using c971_project.Models;
using c971_project.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using c971_project.Services.Firebase;


namespace c971_project.ViewModels
{
    [QueryProperty(nameof(StudentId), "StudentId")]
    public partial class EditStudentViewModel : BaseViewModel
    {

        [ObservableProperty]
        private string studentId;

        [ObservableProperty]
        private Student _student;

        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private Dictionary<string, string> _majorOptions = new();

        [ObservableProperty]
        private string _selectedMajorDisplayName = "Undeclared";

        // Computed property for the Picker
        public List<string> MajorDisplayNames => _majorOptions.Values.ToList();

        public List<string> StatusOptions { get; } = new()
        {
            "Not Enrolled Yet",
            "Currently Enrolled",
            "Paused",
            "Graduated"
        };

        public EditStudentViewModel(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;
            _ = LoadMajorsAsync();
        }

        // This runs when student id changes
        partial void OnStudentIdChanged(string value)
        {
            _= LoadStudentDataAsync(value);
        }

        private async Task LoadStudentDataAsync(string studentId)
        {
            try
            {
                Student = await _firestoreDataService.GetStudentAsync(studentId);

                // Set the picker to the student's current major
                if (Student != null && !string.IsNullOrEmpty(Student.Major))
                {
                    SelectedMajorDisplayName = Student.Major;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading student: {ex.Message}");
            }
        }

        private async Task LoadMajorsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                MajorOptions = await _firestoreDataService.GetMajorsAsync();
                OnPropertyChanged(nameof(MajorDisplayNames));
                // Set default selection
                if (MajorOptions.Count > 0)
                {
                    SelectedMajorDisplayName = MajorDisplayNames.First();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading majors: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Failed to load majors list", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }


        [RelayCommand]
        private async Task SaveStudentAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // set stuends name from picker
                Student.Major = SelectedMajorDisplayName;

                var isTaskValid = await ValidateStudentAsync();

                if (!isTaskValid)
                    return;

                // Save to database
                await _firestoreDataService.SaveStudentAsync(Student);

                // Optional: notify other viewmodels
                WeakReferenceMessenger.Default.Send(new StudentUpdatedMessage());

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
                    nameof(Student.Id),
                    nameof(Student.Name),
                    nameof(Student.Email),
                    nameof(Student.Major)
                );

                await Shell.Current.DisplayAlert("Validation Errors", errorMessage, "OK");
                return false;
            }
            return true;
        }

        [RelayCommand]
        private async Task CancelAsync()
        {
            // Go back one page
            await Shell.Current.GoToAsync("..");
        }


    }
}
