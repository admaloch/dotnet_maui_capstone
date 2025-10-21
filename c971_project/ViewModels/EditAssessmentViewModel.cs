using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Core.Models;
using c971_project.Services.Firebase;
using c971_project.Services.Notifications;
using c971_project.Services.ValidationServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using c971_project.Core.Services;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(AssessmentId), "AssessmentId")]
    public partial class EditAssessmentViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly IScheduleNotificationService _notificationService;
        private readonly IAuthService _authService;
        private string _currentUserId;


        [ObservableProperty]
        private string assessmentId;

        [ObservableProperty]
        private Assessment _assessment;

        [ObservableProperty]
        private ObservableCollection<Assessment> _assessments = new();

        // Change to a property with a private setter
        public List<string> TypeOptions { get; private set; } = new() { "Objective", "Performance" };

        public List<string> StatusOptions { get; } = new()
        {
           "Not started", "In progress", "Completed"
        };

        public EditAssessmentViewModel(IFirestoreDataService firestoreDataService,
            IAuthService authService,
                                     IScheduleNotificationService notificationService)
        {
            _firestoreDataService = firestoreDataService;
            _notificationService = notificationService;
            _authService = authService;
            _currentUserId = _authService.CurrentUserId;
        }

        partial void OnAssessmentIdChanged(string value)
        {
            _ = LoadAssessmentDataAsync(value);
        }

        private async Task LoadAssessmentDataAsync(string assessmentId)
        {
            await LoadAssessmentAsync(assessmentId);
            if (Assessment == null) return;

            await LoadCourseAssessmentsAsync(Assessment.CourseId);

            TypeOptions = Assessments.Count >= 2
                ? new List<string> { Assessment.Type }
                : new List<string> { "Objective", "Performance" };

            OnPropertyChanged(nameof(TypeOptions));
        }

        private async Task LoadAssessmentAsync(string assessmentId)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Assessment = await _firestoreDataService.GetAssessmentAsync(assessmentId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Assessment: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadCourseAssessmentsAsync(string courseId)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                if (string.IsNullOrEmpty(courseId)) return;

                var assessmentList = await _firestoreDataService.GetAssessmentsByCourseIdAsync(courseId);

                Assessments.Clear();
                foreach (var assessment in assessmentList)
                    Assessments.Add(assessment);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading assessments: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task SaveAssessmentAsync()
        {
            if (IsBusy) return;

            Assessment.UserId = _currentUserId;

            try
            {
                IsBusy = true;

                var errors = AssessmentValidator.ValidateAssessment(Assessment).ToString().Trim();
                // print errors if any and return
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }

                // Save to database
                await _firestoreDataService.SaveAssessmentAsync(Assessment);

                // Schedule notifications for the new assessment - ADDED THIS
                var notificationSuccess = await _notificationService.ScheduleAssessmentNotificationsAsync(Assessment);

                // notify update
                WeakReferenceMessenger.Default.Send(new AssessmentUpdatedMessage());

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
    }
}