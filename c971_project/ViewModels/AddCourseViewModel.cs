using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Models;
using c971_project.Services.Data;
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

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(TermId), "TermId")]
    public partial class AddCourseViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly CourseValidator _courseValidator;
        private readonly IScheduleNotificationService _notificationService; // Added

        [ObservableProperty]
        private int termId;

        [ObservableProperty]
        private Course _newCourse;

        [ObservableProperty]
        private Instructor _newInstructor;

        private bool isEdit = false;

        public List<int> CreditUnitOptions { get; } = new()
        {
            1, 2, 3, 4
        };

        // Updated constructor with notification service injection
        public AddCourseViewModel(DatabaseService databaseService,
                                CourseValidator courseValidator,
                                IScheduleNotificationService notificationService) // Added parameter
        {
            _databaseService = databaseService;
            _courseValidator = courseValidator;
            _notificationService = notificationService; // Added
        }

        partial void OnTermIdChanged(int value)
        {
            InitializeDefaultCourse(value);
        }

        private void InitializeDefaultCourse(int termId)
        {
            // Initialize the new course
            NewCourse = new Course
            {
                Name = string.Empty,
                CourseNum = string.Empty,
                CuNum = 3, // default credit units
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(2), // default course length
                DateAdded = DateTime.Now,
                InstructorId = 0,   // will be selected later
                TermId = termId,     // links this course to the current Term
                NotifyStartDate = true, // Default to true
                NotifyEndDate = true    // Default to true
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
        private async Task SaveCourseAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                //  Validate everything
                var errors = await _courseValidator.ValidateCourseFormAsync(TermId, NewCourse, NewInstructor, isEdit);

                //  print errors if any and return
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }

                //  Resolve instructor - if new create new db item - else grab current item
                NewInstructor = await _courseValidator.EnsureInstructorExistsAsync(NewInstructor);

                //  Save course
                await _courseValidator.SaveCourseAsync(TermId, NewCourse, NewInstructor);

                //  Schedule notifications - UPDATED THIS SECTION
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