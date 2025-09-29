using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Text;

namespace c971_project.Helpers
{

    public static class ValidationHelper
    {
        public static string GetErrors<T>(T item, params string[] propertiesToCheck)
            where T : ObservableValidator
        {
            var allErrors = new List<string>();

            foreach (var prop in propertiesToCheck)
            {
                var errors = item.GetErrors(prop);
                if (errors != null)
                {
                    foreach (var err in errors)
                        allErrors.Add(err.ErrorMessage);
                }
            }

            return allErrors.Count > 0
                ? string.Join(Environment.NewLine + Environment.NewLine, allErrors)
                : string.Empty;
        }
        //for adding dates -- date picker prevents past dates, but time picker doesn't prevent earlier in the day so it needs to be validated
        public static string EnsureDateIsNotInPast(DateTime dateTime, String str)
        {
            if (dateTime < DateTime.Now)
                return $"{str} cannot be in the past.";
            return "";
        }

        //for adding dates -- if there are start and end dates -- validate that end is after start
        public static string CheckEndTimeAfterStart(DateTime startTime, DateTime endTime)
        {
            if (endTime <= startTime)
                return "End time must be after start time.";
            return "";
        }

        //many situatnions where start date and end date need to be validated with the above methods in the same way
        public static StringBuilder ValidateStartAndEndDates(DateTime startTime, DateTime endTime)
        {
            var errorBuilder = new StringBuilder();

            errorBuilder.AppendLine(EnsureDateIsNotInPast(startTime, "Start time"));
            errorBuilder.AppendLine(EnsureDateIsNotInPast(endTime, "End time"));
            errorBuilder.AppendLine(CheckEndTimeAfterStart(startTime, endTime));

            return errorBuilder;
        }

    }
}