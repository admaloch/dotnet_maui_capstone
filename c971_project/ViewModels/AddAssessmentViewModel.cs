using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Models;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class AddAssessmentViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly IScheduleNotificationService _notificationService; // Added
        private readonly IAuthService _authService;
        private string _currentUserId;


        [ObservableProperty]
        private string courseId;

        [ObservableProperty]
        private Assessment _newAssessment;

        [ObservableProperty]
        private ObservableCollection<Assessment> _assessments = new();

        public List<string> TypeOptions
        {
            get
            {
                var availableTypes = new List<string>();

                // Check if Objective assessment can be added
                if (CanAddObjectiveAssessment)
                    availableTypes.Add("Objective");

                // Check if Performance assessment can be added  
                if (CanAddPerformanceAssessment)
                    availableTypes.Add("Performance");

                return availableTypes;
            }
        }

        // Helper properties to check limits
        public bool CanAddObjectiveAssessment
            => Assessments.Count(a => a.Type == "Objective") < 1;

        public bool CanAddPerformanceAssessment
            => Assessments.Count(a => a.Type == "Performance") < 1;

        public List<string> StatusOptions { get; } = new()
        {
           "Not started", "In progress", "Completed"
        };

        // Updated constructor with notification service
        public AddAssessmentViewModel(IFirestoreDataService firestoreDataService,
            IAuthService authService,
                                    IScheduleNotificationService notificationService) 
        {
            _firestoreDataService = firestoreDataService;
            _notificationService = notificationService;
            _authService = authService;
            _currentUserId = _authService.CurrentUserId;

            // Update dropdown when assessments collection changes
            Assessments.CollectionChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(TypeOptions));
            };
        }

        partial void OnCourseIdChanged(string value)
        {

            SetDefaultAssessmentItem(value);
            _ = LoadCourseAssessmentsAsync(value);

        }

        private void SetDefaultAssessmentItem(string courseId)
        {
            if (NewAssessment == null)
            {
                NewAssessment = new Assessment
                {
                    Name = string.Empty,
                    UserId = _currentUserId,
                    CourseId = courseId,
                    Type = TypeOptions.FirstOrDefault() ?? string.Empty,
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
                NewAssessment.CourseId = courseId;
            }
        }

        //load course assessments
        private async Task LoadCourseAssessmentsAsync(string courseId)
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;

                var assessmentList = await _firestoreDataService.GetAssessmentsByCourseIdAsync(CourseId);
                Assessments.Clear();
                foreach (var assessment in assessmentList)
                    Assessments.Add(assessment); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading Course: {ex.Message}");
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
            try
            {
                IsBusy = true;
                // validate -- if errors -- print and return
                var errors = AssessmentValidator.ValidateAssessment(NewAssessment).ToString().Trim();
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }
                // Save to database
                await _firestoreDataService.SaveAssessmentAsync(NewAssessment);
                // Schedule notifications for the new assessment - ADDED THIS
                var notificationSuccess = await _notificationService.ScheduleAssessmentNotificationsAsync(NewAssessment);
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