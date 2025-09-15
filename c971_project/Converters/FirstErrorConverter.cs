using CommunityToolkit.Mvvm.ComponentModel;
using System.Globalization;

namespace c971_project.Converters
{
    public class FirstErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableValidator validator && parameter is string propertyName)
            {
                var errors = validator.GetErrors(propertyName);
                if (errors != null)
                {
                    foreach (var err in errors)
                        return err.ErrorMessage; // first error
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
