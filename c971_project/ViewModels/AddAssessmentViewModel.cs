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
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class AddAssessmentViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly IScheduleNotificationService _notificationService; // Added

        [ObservableProperty]
        private int courseId;

        [ObservableProperty]
        private Assessment _newAssessment;

        public List<string> TypeOptions { get; } = new()
        {
            "Objective", "Performance"
        };

        public List<string> StatusOptions { get; } = new()
        {
           "Not started", "In progress", "Completed"
        };

        // Updated constructor with notification service
        public AddAssessmentViewModel(DatabaseService databaseService,
                                    IScheduleNotificationService notificationService) // Added parameter
        {
            _databaseService = databaseService;
            _notificationService = notificationService; // Added
        }

        partial void OnCourseIdChanged(int value)
        {
            Debug.WriteLine($"CourseId set via query: {value}");

            if (NewAssessment == null)
            {
                NewAssessment = new Assessment
                {
                    Name = string.Empty,
                    CourseId = value,
                    Type = "Objective",
                    Status = "In progress",
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddMonths(4),
                    StartTime = new TimeSpan(9, 0, 0),  // 9:00 AM default
                    EndTime = new TimeSpan(17, 0, 0),   // 5:00 PM default
                    NotifyStartDate = true, // Default to true
                    NotifyEndDate = true    // Default to true
                };
            }
            else
            {
                // If NewAssessment exists already, just update the CourseId
                NewAssessment.CourseId = value;
            }
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
                await _databaseService.SaveAssessmentAsync(NewAssessment);

                // Schedule notifications for the new assessment - ADDED THIS
                var notificationSuccess = await _notificationService.ScheduleAssessmentNotificationsAsync(NewAssessment);

                if (!notificationSuccess)
                {
                    Debug.WriteLine("Assessment saved but notifications failed to schedule");
                    // Optional: You could show a subtle warning, but don't block success
                }

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
            NewAssessment.Validate();

            if (NewAssessment.HasErrors)
            {
                var errorMessage = ValidationHelper.GetErrors(
                    NewAssessment,
                    nameof(Assessment.Name)
                );

                await Shell.Current.DisplayAlert("Validation Errors", errorMessage, "OK");
                return false;
            }
            return true;
        }
    }
}