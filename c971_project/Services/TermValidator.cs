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

    public class TermValidator
    {
        private readonly DatabaseService _databaseService;

        public TermValidator(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public static void SetInitialStartAndEndDates(Term term)
        {
            // Ensure StartDate is always the first day of the month at 12:00 AM
            term.StartDate = new DateTime(term.StartDate.Year, term.StartDate.Month, 1, 0, 0, 0);

            // Add 5 months to get to the 6th month, then use the last day
            var endMonth = term.StartDate.AddMonths(5); // Changed from 6 to 5

            // Set EndDate to the last day of that month at 11:59:59 PM
            term.EndDate = new DateTime(
                endMonth.Year,
                endMonth.Month,
                DateTime.DaysInMonth(endMonth.Year, endMonth.Month),
                23, 59, 59
            );
        }

        public static StringBuilder ValidateTerm(Term term)
        {
            term.Validate();

            var errorBuilder = new StringBuilder();

            // term errors
            errorBuilder.AppendLine(ValidationHelper.GetErrors(
                term, nameof(term.Name)));

            return errorBuilder;
        }
    }
}