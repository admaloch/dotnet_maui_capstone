using System;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace c971_project.Converters
{
    public class FirstErrorConverter : IValueConverter
    {
        // value = the Student object
        // parameter = property name (e.g., "Name")
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableValidator validator && parameter is string propertyName)
            {
                var errors = validator.GetErrors(propertyName)?.Cast<System.ComponentModel.DataErrorsChangedEventArgs>().ToList();

                // Actually, ObservableValidator.GetErrors returns IEnumerable<string>
                var firstError = validator.GetErrors(propertyName)?.Cast<string>().FirstOrDefault();
                return firstError ?? string.Empty;
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
