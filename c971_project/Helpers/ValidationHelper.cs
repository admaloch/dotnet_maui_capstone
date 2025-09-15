using CommunityToolkit.Mvvm.ComponentModel;

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
    }
}