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
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using c971_project.Core.Services;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(TermId), "TermId")]
    public partial class AddCourseViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly CourseValidator _courseValidator;
        private readonly IScheduleNotificationService _notificationService;
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string termId;

        [ObservableProperty]
        private Course _newCourse;

        [ObservableProperty]
        private Instructor _newInstructor;

        private string _currentUserId;


        private bool isEdit = false;

        public List<int> CreditUnitOptions { get; } = new()
        {
            1, 2, 3, 4
        };

        // Updated constructor with notification service injection
        public AddCourseViewModel(IFirestoreDataService firestoreDataService,
            IAuthService authService,
                                CourseValidator courseValidator,
                                IScheduleNotificationService notificationService) // Added parameter
        {
            _firestoreDataService = firestoreDataService;
            _courseValidator = courseValidator;
            _notificationService = notificationService; // Added
            _authService = authService;
            _currentUserId = _authService.CurrentUserId;
        }

        partial void OnTermIdChanged(string value)
        {
            InitializeDefaultCourse(value);
        }

        private void InitializeDefaultCourse(string termId)
        {
            // Initialize the new course
            NewCourse = new Course
            {
                Name = string.Empty,
                UserId = _currentUserId,
                CourseNum = string.Empty,
                CuNum = 3, 
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(2), 
                StartTime = new TimeSpan(9, 0, 0),  // 9:00 AM default
                EndTime = new TimeSpan(17, 0, 0),   // 5:00 PM default
                DateAdded = DateTime.Now,
                InstructorId = "0",  
                TermId = termId,     
                NotifyStartDate = true, 
                NotifyEndDate = true    
            };

            // Initialize the new course
            NewInstructor = new Instructor
            {
                Name = string.Empty,
                Email = string.Empty,
                Phone = string.Empty
            };
        }

        [RelayCommand]
        public async Task SaveCourseAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // 1. Validate everything
                var errors = await _courseValidator.ValidateCourseFormAsync(TermId, NewCourse, NewInstructor, isEdit);

                // 2. print errors if any and return
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }

                // 3. Resolve instructor - if new create new db item - else grab current item
                NewInstructor = await _courseValidator.EnsureInstructorExistsAsync(NewInstructor);

                // 4. Save course
                await _courseValidator.SaveCourseAsync(TermId, NewCourse, NewInstructor);

                // 5. Schedule notifications - UPDATED THIS SECTION
                var notificationSuccess = await _notificationService.ScheduleCourseNotificationsAsync(NewCourse);

                if (!notificationSuccess)
                {
                    // Optional: Inform user that notifications failed but course was saved
                    Debug.WriteLine("Course saved but notifications failed to schedule");
                }

                // 6. Notify & navigate
                WeakReferenceMessenger.Default.Send(new CourseUpdatedMessage());

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving Course: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Unable to save Course. Please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}