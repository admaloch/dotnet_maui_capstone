using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Globalization;
using System.Linq;
using Microsoft.Maui.Controls;

namespace c971_project.Converters
{
    public class HasErrorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ObservableValidator validator && parameter is string property)
            {
                return validator.GetErrors(property)?.Cast<System.ComponentModel.DataAnnotations.ValidationResult>().Any() ?? false;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
