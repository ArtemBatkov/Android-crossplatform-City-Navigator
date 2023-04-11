using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Views.Converters
{
    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.ToString() == "ERROR_DECODE_IMAGE")
            {
                return "blank_sight_1.jpg"; // путь к дефолтному изображению
            }
            else
            {                
                return value.ToString(); // значение свойства ImageUrl
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
