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

    public class AssessmentValidator
    {

        public static StringBuilder ValidateAssessment(Assessment assessment)
        {
            assessment.Validate();

            var errorBuilder = new StringBuilder();

            // term errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                assessment, nameof(assessment.Name)));

            // Custom validation rules -- prevent time picker from picking dates in past + ensure past date is after start date
            var pickerDatesErrorBuilder = ValidationHelper.ValidateStartAndEndDates(assessment.StartDateTime, assessment.EndDateTime);
            errorBuilder.Append(pickerDatesErrorBuilder);

            return errorBuilder;
        }

  
    }
}