using c971_project.Models;
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

                // Validate all properties
                NewTerm.ValidateTerm();

                if (NewTerm.HasErrors)
                {
                    // Collect all error messages
                    var propertiesToCheck = new[] { nameof(NewTerm.Name), nameof(NewTerm.StartDate) };
                    var allErrors = new List<string>();

                    foreach (var prop in propertiesToCheck)
                    {
                        var errors = NewTerm.GetErrors(prop);
                        if (errors != null)
                        {
                            foreach (var err in errors)
                                allErrors.Add(err.ErrorMessage);
                        }
                    }

                    var errorMessage = string.Join(Environment.NewLine + Environment.NewLine, allErrors);
                    await App.Current.MainPage.DisplayAlert("Validation Errors", errorMessage, "OK");
                    return;
                }

                // Save to database
                await _databaseService.SaveTermAsync(NewTerm);

                // Optional: send message if HomeViewModel should refresh
                //WeakReferenceMessenger.Default.Send(new TermUpdatedMessage());

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
