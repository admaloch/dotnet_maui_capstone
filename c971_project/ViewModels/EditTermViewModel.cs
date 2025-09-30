using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Models;
using c971_project.Services.Data;
using c971_project.Services.ValidationServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(TermId), "TermId")]
    public partial class EditTermViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int termId;

        [ObservableProperty]
        private Term _term;

        public EditTermViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // This runs when term id changes
        partial void OnTermIdChanged(int value)
        {
            LoadTermDataAsync(value);
        }

        private async Task LoadTermDataAsync(int termId)
        {
            Term = await _databaseService.GetTermByIdAsync(TermId);
        }

        [RelayCommand]
        private async Task SaveTermAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                //validate iinputs
                //TermValidator.SetInitialStartAndEndDates(Term);

                //grab errors and disply message if any
                var errors = TermValidator.ValidateTerm(Term).ToString().Trim();
                //  print errors if any and return
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }

                // Save to database
                await _databaseService.SaveTermAsync(Term);

                // Optional: notify other viewmodels
                WeakReferenceMessenger.Default.Send(new TermUpdatedMessage());

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving term: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to save term. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
