using SightsNavigator.Models;
using SightsNavigator.Services.Web_Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;


namespace SightsNavigator.Services.SightService
{
    internal class SightRequest : ISightRequest
    {
        private string URLCITY = "https://api.opentripmap.com/0.1/en/places/geoname?";

        private string AUTO = "auto";

        public string APIKEY_1 { get => _apikey_1; }
        private string _apikey_1 = "5ae2e3f221c38a28845f05b62f0092f270a88190119b3678a057dd4a";

        
        public string GetCityURL(string name, string country = "auto", string key = "5ae2e3f221c38a28845f05b62f0092f270a88190119b3678a057dd4a")
        {
            if (country == AUTO)
            {
                return $"{URLCITY}name={name}&apikey={key}";
            }
            else
            {
                return $"{URLCITY}name={name}&country={country}&apikey={key}";
            }
        }

        
        public async Task<City> GetCityAsync(string name, string country = "auto")
        {
            string lname = name.ToLower();// lower case name of the city
            string lcountry = country.ToLower();//lower case country
            string url = GetCityURL(lname, lcountry, APIKEY_1).ToLower();

            WebRequest web = new WebRequest();
            HttpResponseMessage response = await web.FetchDataFromAPI(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var converter = JsonConvert.DeserializeObject<dynamic>(json);

                string city_name = converter.name;
                string city_country = converter.country; 
                double city_lat = converter.lat;
                double city_lon = converter.lon;
                long city_population = converter.population;
                string city_timezone = converter.timezone; 
                string city_status = converter.status;

                City city = new City {
                    Name = city_name, 
                    Country = city_country, 
                    Lat = city_lat, 
                    Lon = city_lon, 
                    Population  = city_population,
                    Timezone = city_timezone,
                    Status = city_status
                };

                return city;
            }
            else return null; 
        }
    }
}
