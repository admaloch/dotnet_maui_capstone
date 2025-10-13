using c971_project.Messages;
using c971_project.Models;
using c971_project.Services.Firebase;
using c971_project.Services.Notifications;
using c971_project.Services.ValidationServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Diagnostics;

namespace c971_project.ViewModels
{
    [QueryProperty(nameof(CourseId), "CourseId")]
    public partial class EditCourseViewModel : BaseViewModel
    {
        private readonly IFirestoreDataService _firestoreDataService;
        private readonly CourseValidator _courseValidator;
        private readonly IScheduleNotificationService _notificationService; // Added

        [ObservableProperty]
        private string courseId;

        [ObservableProperty]
        private Course _course;

        [ObservableProperty]
        private Instructor _instructor;

        private bool isEdit = true;

        public List<int> CreditUnitOptions { get; } = new()
        {
            1, 2, 3, 4
        };

        // Updated constructor with notification service
        public EditCourseViewModel(IFirestoreDataService firestoreDataService,
                                 CourseValidator courseValidator,
                                 IScheduleNotificationService notificationService) // Added parameter
        {
            _firestoreDataService = firestoreDataService;
            _courseValidator = courseValidator;
            _notificationService = notificationService; // Added
        }

        partial void OnCourseIdChanged(string value)
        {
            LoadCourseAsync(value); // call async fire-and-forget
        }

        private async Task LoadCourseAsync(string courseId)
        {
            Course = await _firestoreDataService.GetCourseAsync(courseId);
            Instructor = await _firestoreDataService.GetInstructorAsync(Course.InstructorId);
        }

        [RelayCommand]
        private async Task SaveCourseAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // 1. Validate everything
                var errors = await _courseValidator.ValidateCourseFormAsync(Course.TermId, Course, Instructor, isEdit);

                // 2. print errors if any and return
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors", errors, "OK");
                    return;
                }

                // 3. Resolve instructor - if new create new db item - else grab current item
                Instructor = await _courseValidator.EnsureInstructorExistsAsync(Instructor);

                // 4. Save course
                await _courseValidator.SaveCourseAsync(Course.TermId, Course, Instructor);

                // 5. Schedule notifications for the UPDATED course - ADDED THIS
                var notificationSuccess = await _notificationService.ScheduleCourseNotificationsAsync(Course);

                if (!notificationSuccess)
                {
                    // Optional: Log that notifications failed but course was saved
                    Debug.WriteLine("Course saved but notifications failed to update");
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