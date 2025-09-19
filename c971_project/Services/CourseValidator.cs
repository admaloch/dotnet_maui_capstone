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

namespace c971_project.Services
{

    public class CourseValidator
    {
        private readonly DatabaseService _databaseService;

        public CourseValidator(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        //handle validation for toolkit, custom, instructor, unique course etc.. 
        public async Task<string> ValidateCourseFormAsync(int TermId, Course NewCourse, Instructor NewInstructor, bool isEdit)
        {
            var errorBuilder = new StringBuilder();

            // toolkit validation for instructor and course model
            NewCourse.Validate();
            NewInstructor.Validate();

            // Course errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                NewCourse, nameof(Course.Name), nameof(Course.CourseNum),
                nameof(Course.CuNum), nameof(Course.StartDate), nameof(Course.EndDate)));

            // Custom rules
            if (NewCourse.EndDate < NewCourse.StartDate)
                errorBuilder.AppendLine("End date cannot be before start date.");

            // Instructor errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                NewInstructor, nameof(Instructor.Name), nameof(Instructor.Phone), nameof(Instructor.Email)));

            // Check unique course number
            var error = await ValidateUniqueCourseNumAsync(NewCourse, isEdit);
            if (!string.IsNullOrEmpty(error))
                errorBuilder.AppendLine(error);

            return errorBuilder.ToString().Trim();
        }


        public async Task<string> ValidateUniqueCourseNumAsync(Course course, bool isEdit = false)
        {
            var existingCourse = await _databaseService.GetCourseByCourseNumAsync(course.CourseNum);

            if (existingCourse != null)
            {
                // If editing, allow the same course to keep its number
                if (isEdit && existingCourse.CourseId == course.CourseId)
                    return "";

                return "A course with this course number already exists.";
            }
            return "";
        }

        public async Task EnsureInstructorExistsAsync(Instructor NewInstructor)
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


        public async Task SaveCourseAsync(int TermId, Course NewCourse, Instructor NewInstructor)
        {
            NewCourse.TermId = TermId;
            NewCourse.InstructorId = NewInstructor.InstructorId;
            await _databaseService.SaveCourseAsync(NewCourse);
        }
    }
}