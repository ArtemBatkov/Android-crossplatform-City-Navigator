namespace SightsNavigator.Views.Converters
{
    using SightsNavigator.Models;
    using System;
    using System.Globalization;



    public class TitleConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not City.Sight sight)
                return String.Empty;

            if (!String.IsNullOrEmpty(sight.Name))
                return $"{sight.Name}";
            else if (sight.SightAddress != null && !String.IsNullOrEmpty(sight.SightAddress.FullAddress))
                return $"{sight.SightAddress.FullAddress}";
            else
                return "Unknown";
            //var name = values[0] as string;
            //var address = values[1] as City.Sight.Address;

            //var sight = values[2] as City.Sight;


            //return name.ToString();
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
