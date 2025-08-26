using c971_project.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using c971_project.Services;



namespace c971_project.ViewModels
{
    public partial class StudentViewModel : BaseViewModel
    {
        [ObservableProperty]
        private Student student;

        private readonly DatabaseService _databaseService;

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
            Debug.WriteLine($"Saving student: {Student?.Name}");

            // TODO: save to database (you can inject DatabaseService if needed)
            await Shell.Current.GoToAsync("..");
        }
    }
}
