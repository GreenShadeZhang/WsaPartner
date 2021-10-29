using System;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace WsaPartner.Helpers
{
    public class BoolToEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }
            else
            {
                var ret = (bool)value;

                if (!ret)
                {
                    return Visibility.Collapsed;
                }
            }

            return Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
