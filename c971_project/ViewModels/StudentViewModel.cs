using c971_project.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using c971_project.Services;

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
        private async Task SaveAsync()
        {
            Debug.WriteLine($"[StudentViewModel] Saving student: {Student?.Name}");

            // TODO: save to database
            await Shell.Current.GoToAsync("..");
        }
    }
}
