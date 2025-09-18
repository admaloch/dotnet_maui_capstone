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

                // Build error message incrementally
                var errorBuilder = new StringBuilder();

                // Validate data annotations
                NewInstructor.Validate();
                NewCourse.Validate();

                // Add instructor errors
                var instructorErrors = ValidationHelper.GetErrors(
                    NewInstructor,
                    nameof(Instructor.Name),
                    nameof(Instructor.Phone),
                    nameof(Instructor.Email)
                );
                if (!string.IsNullOrEmpty(instructorErrors))
                {
                    errorBuilder.AppendLine(instructorErrors);
                }

                // Add course errors
                var courseErrors = ValidationHelper.GetErrors(
                    NewCourse,
                    nameof(Course.Name),
                    nameof(Course.CourseNum),
                    nameof(Course.CuNum),
                    nameof(Course.StartDate),
                    nameof(Course.EndDate)
                );
                if (!string.IsNullOrEmpty(courseErrors))
                {
                    errorBuilder.AppendLine(courseErrors);
                }

                // Add custom validations
                if (NewCourse.EndDate < NewCourse.StartDate)
                {
                    errorBuilder.AppendLine("End date cannot be before start date.");
                }

                // Check course number uniqueness
                var existingCourse = await _databaseService.GetCourseByCourseNumAsync(NewCourse.CourseNum);
                if (existingCourse != null)
                {
                    errorBuilder.AppendLine("A course with this course number already exists.");
                }

                // Display all errors at once
                if (errorBuilder.Length > 0)
                {
                    await Shell.Current.DisplayAlert("Validation Errors",
                        $"Please fix the following errors:\n\n{errorBuilder}", "OK");
                    return;
                }

                //check if instfuctor already exists
                var searchInstructor = await _databaseService.GetInstructorByEmailAsync(NewInstructor.Email);
                if (searchInstructor != null)
                {
                    Debug.WriteLine("New instructor created");
                    //set the instructor to the existing one
                    NewInstructor = searchInstructor;
                }
                else
                {
                    //save the new instructor
                    Debug.WriteLine($"Instructor already found -- set to {searchInstructor.Name}");

                    await _databaseService.SaveInstructorAsync(NewInstructor);
                }

                NewCourse.TermId = TermId;
                NewCourse.InstructorId = NewInstructor.InstructorId;


                await _databaseService.SaveCourseAsync(NewCourse);

                // Optional: notify other viewmodels
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

    }
}
