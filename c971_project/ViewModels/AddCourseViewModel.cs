using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Models;
using c971_project.Services;
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

        [ObservableProperty]
        private int termId;

        [ObservableProperty]
        private Course _newCourse;

        [ObservableProperty]
        private Instructor _newInstructor;

        public List<int> CreditUnitOptions { get; } = new()
        {
            1, 2, 3, 4
        };



        public AddCourseViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

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
                TermId = termId     // links this course to the current Term
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
        private async Task OnSaveCourseAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;

                // 1. Validate everything
                var errors = await ValidateCourseAndInstructorAsync();
                if (!string.IsNullOrWhiteSpace(errors))
                {
                    await Shell.Current.DisplayAlert("Validation Errors",
                        $"Please fix the following errors:\n\n{errors}", "OK");
                    return;
                }

                // 2. Resolve instructor (existing or new)
                await EnsureInstructorExistsAsync();

                // 3. Save course
                await SaveCourseAsync();

                // 4. Notify & navigate
                WeakReferenceMessenger.Default.Send(new CourseUpdatedMessage());
                await Shell.Current.DisplayAlert("Success", "Course saved successfully.", "OK");
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


        private async Task<string> ValidateCourseAndInstructorAsync()
        {
            var errorBuilder = new StringBuilder();

            // Data annotation validation
            NewInstructor.Validate();
            NewCourse.Validate();

            // Instructor errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                NewInstructor, nameof(Instructor.Name), nameof(Instructor.Phone), nameof(Instructor.Email)));

            // Course errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                NewCourse, nameof(Course.Name), nameof(Course.CourseNum),
                nameof(Course.CuNum), nameof(Course.StartDate), nameof(Course.EndDate)));

            // Custom rules
            if (NewCourse.EndDate < NewCourse.StartDate)
                errorBuilder.AppendLine("End date cannot be before start date.");

            // Check unique course number
            var existingCourse = await _databaseService.GetCourseByCourseNumAsync(NewCourse.CourseNum);
            if (existingCourse != null)
                errorBuilder.AppendLine("A course with this course number already exists.");

            return errorBuilder.ToString().Trim();
        }

        private async Task EnsureInstructorExistsAsync()
        {
            var searchInstructor = await _databaseService.GetInstructorByEmailAsync(NewInstructor.Email);

            if (searchInstructor != null)
            {
                Debug.WriteLine($"Instructor already found -- set to {searchInstructor.Name}");
                NewInstructor = searchInstructor;
            }
            else
            {
                Debug.WriteLine("New instructor created");
                await _databaseService.SaveInstructorAsync(NewInstructor);
            }
        }

        private async Task SaveCourseAsync()
        {
            NewCourse.TermId = TermId;
            NewCourse.InstructorId = NewInstructor.InstructorId;
            await _databaseService.SaveCourseAsync(NewCourse);
        }


    }
}
