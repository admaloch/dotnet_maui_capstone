using c971_project.Models;
using c971_project.Views;
using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using c971_project.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using c971_project.Messages;


namespace c971_project.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Student _currentStudent;

        [ObservableProperty]
        private ObservableCollection<Term> _terms = new();

        public HomeViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            _ = LoadDataAsync();

            WeakReferenceMessenger.Default.Register<StudentUpdatedMessage>(this, async (r, m) =>
            {
                await LoadStudentAsync(); // reload from DB so Home reflects persisted data
            });

            WeakReferenceMessenger.Default.Register<TermUpdatedMessage>(this, async (r, m) =>
            {
                await LoadTermAsync(); // refresh terms from DB
            });
        }

        public async Task LoadDataAsync()
        {
            await LoadStudentAsync();
            await LoadTermAsync();
        }

        public async Task LoadStudentAsync()
        {
            try
            {
                CurrentStudent = await _databaseService.GetCurrentStudentAsync();
            }
            catch (Exception ex)
            {
                CurrentStudent = null;
                Debug.WriteLine($"Debug msg: Error loading student data: {ex.Message}");

            }
        }

        public async Task LoadTermAsync()
        {
            try
            {
                var terms = await _databaseService.GetTermsAsync();

                Terms.Clear();
                foreach (var term in terms)
                {
                    Terms.Add(term);
                }
                Debug.WriteLine($"number of terms is: {Terms.Count}");

            }
            catch (Exception ex)
            {
                Terms.Clear();
                Debug.WriteLine($"Debug msg: Error loading term data: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task OnAddTermAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(AddTermPage));
            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task OnDeleteTermAsync(Term term)
        {
            if (IsBusy || term == null) return;

            bool confirm = await Shell.Current.DisplayAlert(
                "Delete Term",
                $"Are you sure you want to delete '{term.Name}'?",
                "Delete",
                "Cancel");

            if (!confirm) return;

            try
            {
                IsBusy = true;
                //also delete courses - notes - assessments
                var courses = await _databaseService.GetCoursesByTermIdAsync(term.TermId);
                foreach (var course in courses)
                {
                    await _databaseService.DeleteAssessmentsByCourseIdAsync(course.CourseId);
                    await _databaseService.DeleteNotesByCourseIdAsync(course.CourseId);
                    await _databaseService.DeleteCourseAsync(course);
                }
                await _databaseService.DeleteTermAsync(term);
                Terms.Remove(term);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            finally { IsBusy = false; }

        }

        [RelayCommand]
        private async Task OnEditStudentAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(EditStudentPage),
                    new Dictionary<string, object> { { "StudentId", CurrentStudent.StudentId } });

            }
            finally { IsBusy = false; }
        }

        [RelayCommand]
        private async Task OnTermPageAsync(Term term)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(TermPage),
                    new Dictionary<string, object> { { "TermId", term.TermId } });
            }
            finally { IsBusy = false; }
        }


    }
}
