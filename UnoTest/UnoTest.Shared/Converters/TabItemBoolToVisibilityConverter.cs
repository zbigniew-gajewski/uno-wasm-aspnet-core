using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace UnoTest.Shared.Converters
{
    public class TabItemBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var selectedString = value as string;
            var visibilityParameter = parameter as string;

            if (selectedString != null && visibilityParameter != null && selectedString == visibilityParameter)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
