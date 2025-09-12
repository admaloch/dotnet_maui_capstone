using c971_project.Models;
using c971_project.Views;
using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using c971_project.ViewModels;
using c971_project.Messages;
using CommunityToolkit.Mvvm.Messaging;



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
            Title = "Home";

            // Load initial data
            _ = LoadDataAsync();

            // Listen for term updates
            WeakReferenceMessenger.Default.Register<TermUpdatedMessage>(this, async (r, m) =>
            {
                await LoadDataAsync();
            });

            // Listen for student updates
            WeakReferenceMessenger.Default.Register<StudentUpdatedMessage>(this, async (r, m) =>
            {
                await LoadDataAsync();
            });
        }

        public async Task LoadDataAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                CurrentStudent = await _databaseService.GetCurrentStudentAsync();
                var termList = await _databaseService.GetTermsAsync();

                Terms.Clear();
                foreach (var term in termList)
                {
                    Terms.Add(term); // ObservableCollection will update UI automatically
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading HomeViewModel data: {ex.Message}");
                CurrentStudent = null;
                Terms.Clear();
            }
            finally
            {
                IsBusy = false;
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
        private async Task OnEditStudentAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.GoToAsync(nameof(EditStudentPage),
                    new Dictionary<string, object> { { "Student", CurrentStudent } });
            }
            finally { IsBusy = false; }
        }
    }
}
