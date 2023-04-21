using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Models
{
    public static class TripListModel
    {
        private static List<City> _trips = new List<City>();

        public static void AddTrip(City city)
        {
            if (_trips.Any(c => c.Name == city.Name))
            {
                return;
            }
            else
            {
                _trips.Insert(0, city);
            }
        }

        public static List<City> GetTripList() {
            return _trips;
        }

        public static int getTripLength()
        {
            return _trips.Count;
        }

        public static void updateCityProporties(City city)
        {
            int index = _trips.FindIndex(c => c.Name == city.Name);            
            if (index != -1)
            {                
                _trips[index] = city;
            }
        }

        public static bool CityExists(City city)
        {
            return _trips.Any(c => c.Name.Equals(city.Name));
        }

        public static City getCityByName(string name){
            var city = _trips.FirstOrDefault(c => c.Name.Equals(name));
            return city;
        }
    }
}
