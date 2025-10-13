using c971_project.Models;
using c971_project.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using c971_project.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using c971_project.Messages;
using c971_project.Services.Firebase;


namespace c971_project.ViewModels
{
    public partial class HomeViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly AuthService _authService;
        private string _currentUserId;


        [ObservableProperty]
        private Student _currentStudent;

        [ObservableProperty]
        private ObservableCollection<Term> _terms = new();

        // Computed property for button state
        public bool CanAddMoreTerms => Terms.Count < 6;

        public HomeViewModel(IFirestoreDataService firestoreDataService, AuthService authService)
        {
            _firestoreDataService = firestoreDataService;
            _authService = authService;
            _currentUserId = _authService.CurrentUserId;

            _ = LoadDataAsync();

            // Notify when Terms collection changes
            Terms.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(CanAddMoreTerms));
            };

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
                CurrentStudent = await _firestoreDataService.GetStudentAsync(_currentUserId); //need user id
            }
            catch (Exception ex)
            {
                CurrentStudent = null;
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");

            }
        }

        public async Task LoadTermAsync()
        {
            try
            {
                var terms = await _firestoreDataService.GetTermsAsync(_currentUserId); //user id
                Terms.Clear();
                foreach (var term in terms)
                {
                    Terms.Add(term);
                }

            }
            catch (Exception ex)
            {
                Terms.Clear();
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
            // update on add courses
            OnPropertyChanged(nameof(CanAddMoreTerms));
        }

        [RelayCommand]
        private async Task OnAddTermAsync()
        {
            if (IsBusy) return;
            if (!CanAddMoreTerms)
            {
                await Shell.Current.DisplayAlert("Notification", "You have reached the maximum 6 terms", "OK");
                return;
            }
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
                var courses = await _firestoreDataService.GetCoursesByTermIdAsync(term.Id);
                foreach (var course in courses)
                {
                    await _firestoreDataService.DeleteAssessmentsByCourseIdAsync(course.Id);
                    await _firestoreDataService.DeleteNotesByCourseIdAsync(course.Id);
                    await _firestoreDataService.DeleteCourseAsync(course.Id);
                }
                await _firestoreDataService.DeleteTermAsync(term.Id);
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
                    new Dictionary<string, object> { { "StudentId", CurrentStudent.Id } });

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
                    new Dictionary<string, object> { { "TermId", term.Id } });
            }
            finally { IsBusy = false; }
        }


    }
}
