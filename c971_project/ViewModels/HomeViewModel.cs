using c971_project.Models;
using c971_project.Views;
using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using c971_project.ViewModels;


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

            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            try
            {
                CurrentStudent = await _databaseService.GetCurrentStudentAsync();
                Terms = new ObservableCollection<Term>(await _databaseService.GetTermsAsync());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading HomeViewModel data: {ex.Message}");
                CurrentStudent = null;
                var termList = await _databaseService.GetTermsAsync();
                // Clear the collection first (optional depending on use case)
                Terms.Clear();
                foreach (var term in termList)
                {
                    term.Name = term.Name.ToUpper(); // just an example update
                    Terms.Add(term); // this will notify UI because ObservableCollection
                }
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
