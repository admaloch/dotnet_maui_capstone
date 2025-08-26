using c971_project.Models;
using c971_project.Services;
using c971_project.Views;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace c971_project.ViewModels
{
    public partial class HomeViewModel : ObservableObject
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Student _currentStudent = new();

        [ObservableProperty]
        private ObservableCollection<Term> _terms = new();

        public HomeViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadStudentDataCommand.ExecuteAsync(null);
        }

        [RelayCommand]
        private async Task LoadStudentDataAsync()
        {
            try
            {
                var students = await _databaseService.GetStudentsAsync();
                if (students != null && students.Count > 0)
                {
                    CurrentStudent = students[0];
                    await LoadTermsForCurrentStudentAsync();
                }
                else
                {
                    CurrentStudent = new Student
                    {
                        StudentId = "N/A",
                        Name = "No Student Data",
                        Email = "N/A",
                        Status = "Not Enrolled",
                        Major = "N/A"
                    };
                    Terms.Clear();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading student data: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task LoadTermsForCurrentStudentAsync()
        {
            if (string.IsNullOrEmpty(CurrentStudent?.StudentId))
                return;

            try
            {
                var terms = await _databaseService.GetTermsByStudentIdAsync(CurrentStudent.StudentId);
                Terms = new ObservableCollection<Term>(terms ?? new List<Term>());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading terms: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task EditStudentAsync()
        {
            if (CurrentStudent != null && !string.IsNullOrEmpty(CurrentStudent.StudentId))
            {
                await Shell.Current.GoToAsync($"{nameof(EditStudentPage)}?studentId={CurrentStudent.StudentId}");
            }
            else
            {
                await Shell.Current.GoToAsync(nameof(EditStudentPage));
            }
        }

        [RelayCommand]
        private async Task AddTermAsync()
        {
            Debug.WriteLine("Add Term clicked");
            // await Shell.Current.GoToAsync(nameof(AddEditTermPage));
        }
    }
}