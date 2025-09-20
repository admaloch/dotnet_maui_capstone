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
    [QueryProperty(nameof(AssessmentId), "AssessmentId")]
    public partial class EditAssessmentViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        [ObservableProperty]
        private int assessmentId;

        [ObservableProperty]
        private Assessment _assessment;

        public List<string> TypeOptions { get; } = new()
        {
            "Objective", "Performance"
        };

        public List<string> StatusOptions { get; } = new()
        {
           "Not started", "In progress", "Completed"
        };

        public EditAssessmentViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        // This runs when CourseId is assigned by Shell after navigation
        partial void OnAssessmentIdChanged(int value)
        {
            LoadAssessmentDataAsync(value);
        }

        private async Task LoadAssessmentDataAsync(int assessmentId)
        {
            Assessment = await _databaseService.GetAssessmentByIdAsync(AssessmentId);
        }

        [RelayCommand]
        private async Task SaveAssessmentAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                var isTaskValid = await ValidateAssessmentAsync();

                if (!isTaskValid)
                    return;

                // Save to database
                await _databaseService.SaveAssessmentAsync(Assessment);

                // Optional: notify other viewmodels
                WeakReferenceMessenger.Default.Send(new AssessmentUpdatedMessage());

                await Shell.Current.DisplayAlert("Success", "Assessment saved successfully.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving assessment: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to save assessment. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task<bool> ValidateAssessmentAsync()
        {
            Assessment.Validate();

            if (Assessment.HasErrors)
            {
                var errorMessage = ValidationHelper.GetErrors(
                    Assessment,
                    nameof(Assessment.Name)
                );

                await Shell.Current.DisplayAlert("Validation Errors", errorMessage, "OK");
                return false;
            }
            return true;
        }

    }
}
