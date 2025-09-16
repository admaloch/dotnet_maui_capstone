using c971_project.Messages;
using c971_project.Models;
using c971_project.Services;
using c971_project.ViewModels;
using c971_project.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace c971_project.ViewModels
{

    [QueryProperty(nameof(TermId), "TermId")]
    public partial class TermViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int termId;

        [ObservableProperty]
        private Term term;

        public TermViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // Called automatically when TermId is set by Shell
        partial void OnTermIdChanged(int value)
        {
            _ = LoadTermAsync(value);
        }

        private async Task LoadTermAsync(int id)
        {
            try
            {
                Term = await _databaseService.GetTermByIdAsync(id);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Term: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task OnAddCourseAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                await Shell.Current.DisplayAlert("Success", "Clicked to add course", "OK");
            }
            finally { IsBusy = false; }
        }
    }

}
