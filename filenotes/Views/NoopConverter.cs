using System;
using Windows.UI.Xaml.Data;

namespace Sbs20.Filenotes.Views
{
    // See: http://stackoverflow.com/questions/30359025/windows-10-xbind-to-selecteditem
    public class NoopConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value;
        }
    }
}
