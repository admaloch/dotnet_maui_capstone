using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Core.Models;
using c971_project.Services.Firebase;
using c971_project.Services.ValidationServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using c971_project.Core.Services;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(TermId), "TermId")]
    public partial class EditTermViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private string termId;

        [ObservableProperty]
        private Term _term;

        public EditTermViewModel(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;
        }

        // This runs when term id changes
        partial void OnTermIdChanged(string value)
        {
            _ = LoadTermDataAsync(value);
        }

        private async Task LoadTermDataAsync(string termId)
        {
            Term = await _firestoreDataService.GetTermAsync(termId);
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
                await _firestoreDataService.SaveTermAsync(Term);

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
