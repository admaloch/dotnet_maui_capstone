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
            Title = "Home";

            _ = LoadDataAsync();

            WeakReferenceMessenger.Default.Register<StudentUpdatedMessage>(this, async (r, m) =>
            {
                await LoadDataAsync(); // reload from DB so Home reflects persisted data
            });

            WeakReferenceMessenger.Default.Register<TermUpdatedMessage>(this, async (r, m) =>
            {
                await LoadDataAsync(); // refresh terms from DB
            });
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
                    new Dictionary<string, object> { { "Student", CurrentStudent.Clone() } });
            }
            finally { IsBusy = false; }
        }




    }
}
