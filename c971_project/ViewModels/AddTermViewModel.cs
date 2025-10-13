using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Models;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace c971_project.ViewModels
{
    public partial class AddTermViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;

        [ObservableProperty]
        private Term _newTerm;


        public AddTermViewModel(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;

            // Initialize the new term
            NewTerm = new Term
            {
                Name = string.Empty,
                StartDate = DateTime.Today.AddDays(1),
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

                //validate iinputs // code for 
                //TermValidator.SetInitialStartAndEndDates(NewTerm);

                //grab errors and disply message if any
                var errors = TermValidator.ValidateTerm(NewTerm).ToString().Trim();
                //  print errors if any and return
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }

                // Save to database
                await _firestoreDataService.SaveTermAsync(NewTerm);

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
