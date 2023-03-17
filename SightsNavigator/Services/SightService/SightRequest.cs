using SightsNavigator.Models;
using SightsNavigator.Services.Web_Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json;
using System.Globalization;
using System.Diagnostics.Metrics;
using System.Timers;


namespace SightsNavigator.Services.SightService
{
    internal class SightRequest : ISightRequest
    {
        private static System.Timers.Timer RequesTimer;
        private static double _timerPeriod = 1000;
        public SightRequest()
        {
            RequesTimer = new System.Timers.Timer(_timerPeriod);
            RequesTimer.Elapsed += async (sender, e) => await HandleTimer();

        }

   

        public async Task<City> GetCityAsync(string name, string country = "auto")
        {
            //Step 1
           //Get default city information 
            City city = await GetBasicCityInfoAsync(name,country);
            
            //Step 2
            if (city is null) return null;
            //Get list of Xids of the city 
            var radius = 30_000; // HARDCODED
            var xids = await GetSightsListOfCityAsync(city, radius);
            //insert the list inside of city object
            if (xids is null) return null;
            city.ListOfXids = xids.ToList<String>();

            //Step 3
            //Get List of sights
            await GetListOfSights(city);


            return city;
        }


    

        private async Task<City> GetBasicCityInfoAsync(string name, string country = "auto")
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

                City city = new City
                {
                    Name = city_name,
                    Country = city_country,
                    Lat = city_lat,
                    Lon = city_lon,
                    Population = city_population,
                    Timezone = city_timezone,
                    Status = city_status
                };

                return city;
            }
            else return null;
        }

        public async Task<IEnumerable<String>> GetSightsListOfCityAsync(City city, double radius)
        {
            if (city is null)
            {
                return null;
            }
            string txt = city.Lon.ToString(CultureInfo.InvariantCulture);
            double lon = double.Parse(txt, CultureInfo.InvariantCulture);
            txt = city.Lat.ToString(CultureInfo.InvariantCulture);
            double lat = double.Parse(txt, CultureInfo.InvariantCulture);
            string url = GetSightsListURL(lon, lat, radius);

            WebRequest web = new WebRequest();
            HttpResponseMessage response = await web.FetchDataFromAPI(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var converter = JsonConvert.DeserializeObject<dynamic>(json);

                var features = converter.features;

                List<String> Xids = new List<String>();

                foreach (var feature in features)
                {
                    var property = feature.properties;
                    var xid = property.xid.ToString();
                    Xids.Add(xid);
                }
                return Xids;
            }
            else return null;
        }

        private async Task<IEnumerable<City.Sight>> GetListOfSights(City city)
        {
            if (city is null) return null;
            if (city.ListOfXids is null) return null;
            if (city.ListOfXids.Count() == 0) return null; 

            var xids = city.ListOfXids; ;
            string url;

            //RequesTimer.Start();

            int start = 0;
            int step = 5;
            int end = xids.Count();

            string xid;

            WebRequest web = new WebRequest();
            HttpResponseMessage response;

            City.Sight blankSight = new City.Sight();

            var tasks = new List<Task<HttpResponseMessage>>();
            var responses = new List<HttpResponseMessage>();

            var sights = new List<City.Sight>();
            watch.Start();

            while (start < end)
            {
                try
                {
                    if (Math.Abs(start - end) < step) step = Math.Abs(start - end);
                    else step = 5;

                    var endchunk = (step + start > end) ? end : step + start;

                    for (int i = start; i < endchunk; i++)
                    {
                        xid = xids[i];
                        url = GetSightURL(xid);
                        tasks.Add(web.FetchDataFromAPI(url));
                    }

                    await Task.WhenAll(tasks.ToArray());

                    foreach(var task in tasks)
                    {
                        responses.Add(task.Result);
                    }

                    var sightsObjects = await DecodeSight(responses);
                    foreach(var sight in sightsObjects)
                    {
                        if(sight!=null) sights.Add(sight);
                    }

                    //await Task.Delay((int)_timerPeriod);
                    Thread.Sleep(1500);
                    if (start + step > end) break;
                    else start += step;
                    tasks.Clear();
                    responses.Clear();
                     
                }
                catch (Exception ex) { }

            }
            watch.Stop();
            System.Diagnostics.Debug.WriteLine($"Execution Time: {watch.Elapsed} s");
            return sights;
        }


        private async Task<List<City.Sight>> DecodeSight(List<HttpResponseMessage> responses)
        {
            var sights = new List<City.Sight>();    
            foreach(var response in responses)
            {
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var converter = JsonConvert.DeserializeObject<dynamic>(json);
                    var sight = new City.Sight();
                    sight.Xid = converter.xid;
                    sight.Name = converter.name;
                    sights.Add(sight);
                }
                else sights.Add(null);
            }
            return sights;            
        }






        private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

        private async Task HandleTimer()
        {
            watch.Stop();
            System.Diagnostics.Debug.WriteLine($"Execution Time: {watch.Elapsed} s");
            
        }








        



        private string URLSIGHT = "https://api.opentripmap.com/0.1/en/places/xid/";

        private string URLCITY = "https://api.opentripmap.com/0.1/en/places/geoname?";
        private string URLSIGHTSLIST = "https://api.opentripmap.com/0.1/en/places/radius?";

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

        public string GetSightsListURL(double lon, double lat, double radius, string key = "5ae2e3f221c38a28845f05b62f0092f270a88190119b3678a057dd4a")
        {

            return $"{URLSIGHTSLIST}radius={radius.ToString(CultureInfo.InvariantCulture)}" +
            $"&lon={lon.ToString(CultureInfo.InvariantCulture)}&" +
            $"lat={lat.ToString(CultureInfo.InvariantCulture)}&" +
            $"apikey={key.ToString(CultureInfo.InvariantCulture)}";
        }

        
        public string GetSightURL(string xid, string key = "5ae2e3f221c38a28845f05b62f0092f270a88190119b3678a057dd4a")
        {
            if (String.IsNullOrEmpty(xid)) return "not correct link";
            else return $"{URLSIGHT}{xid}?apikey={key}";
        }

        public static void ChangeTimerPeriod(double newPeriod)
        {
            RequesTimer.Stop();
            _timerPeriod = newPeriod;
            RequesTimer.Interval = newPeriod;
        }
    }
}
