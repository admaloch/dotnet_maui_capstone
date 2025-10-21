using c971_project.Helpers;
using c971_project.Messages;
using c971_project.Core.Models;
using c971_project.Services.Firebase;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using c971_project.Core.Services;

namespace c971_project.Services.ValidationServices
{

    public class CourseValidator
    {
        private readonly IFirestoreDataService _firestoreDataService;

        public CourseValidator(IFirestoreDataService firestoreDataService)
        {
            _firestoreDataService = firestoreDataService;
        }

        //handle validation for toolkit, custom, instructor, unique course etc.. 
        public async Task<string> ValidateCourseFormAsync(string TermId, Course Course, Instructor Instructor, bool isEdit)
        {
            var errorBuilder = new StringBuilder();

            // toolkit validation for instructor and course model
            Course.Validate();

            Instructor.Validate();

            // Course errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                Course, nameof(Course.Name), nameof(Course.CourseNum),
                nameof(Course.CuNum), nameof(Course.StartDate), nameof(Course.EndDate)));

            // Custom validation rules -- prevent time picker from picking dates in past + ensure past date is after start date
            var pickerDatesErrorBuilder = ValidationHelper.ValidateStartAndEndDates(Course.StartDateTime, Course.EndDateTime);
            errorBuilder.Append(pickerDatesErrorBuilder);

            // Instructor errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                Instructor, nameof(Instructor.Name), nameof(Instructor.Phone), nameof(Instructor.Email)));

            // Check unique course number
            var error = await ValidateUniqueCourseNumAsync(Course, isEdit);
            if (!string.IsNullOrEmpty(error))
                errorBuilder.AppendLine(error);

            return errorBuilder.ToString().Trim();
        }


        public async Task<string> ValidateUniqueCourseNumAsync(Course course, bool isEdit = false)
        {
            var existingCourse = await _firestoreDataService.GetCourseByCourseNumAsync(course.CourseNum);

            if (existingCourse != null)
            {
                // If editing, allow the same course to keep its number
                if (isEdit && existingCourse.Id == course.Id)
                    return "";

                return "A course with this course number already exists.";
            }
            return "";
        }


        public async Task<Instructor> EnsureInstructorExistsAsync(Instructor Instructor)
        {
            var searchInstructor = await _firestoreDataService.GetInstructorByEmailAsync(Instructor.Email);

            if (searchInstructor != null)
            {
                Debug.WriteLine($"Instructor already found -- set to {searchInstructor.Name}");
                return searchInstructor;
            }
            else
            {
                Debug.WriteLine("New instructor created");
                await _firestoreDataService.SaveInstructorAsync(Instructor);
                return Instructor;
            }
        }

        public async Task SaveCourseAsync(string TermId, Course Course, Instructor Instructor)
        {
            Course.TermId = TermId;
            Course.InstructorId = Instructor.Id;
            await _firestoreDataService.SaveCourseAsync(Course);
        }
    }
}