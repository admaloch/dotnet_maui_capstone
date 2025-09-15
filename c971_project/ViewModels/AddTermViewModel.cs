using c971_project.Models;
using c971_project.Helpers;
using c971_project.Messages;

using c971_project.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace c971_project.ViewModels
{
    public partial class AddTermViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private Term _newTerm;

        public AddTermViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            // Initialize the new term
            NewTerm = new Term
            {
                Name = string.Empty,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(4) // default length
            };
        }

        [RelayCommand]
        private async Task SaveTermAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                NewTerm.Validate();

                if (NewTerm.HasErrors)
                {
                    var errorMessage = ValidationHelper.GetErrors(
                        NewTerm,
                        nameof(Term.Name),
                        nameof(Term.StartDate)
                    );

                    await Shell.Current.DisplayAlert("Validation Errors", errorMessage, "OK");
                    return;
                }

                // Save to database
                await _databaseService.SaveTermAsync(NewTerm);

                // Optional: notify other viewmodels
                WeakReferenceMessenger.Default.Send(new TermUpdatedMessage());

                await Shell.Current.DisplayAlert("Success", "Term saved successfully.", "OK");
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
