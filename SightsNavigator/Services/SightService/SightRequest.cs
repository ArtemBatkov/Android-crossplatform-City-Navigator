using Newtonsoft.Json;
using SightsNavigator.Models;
using SightsNavigator.Services;
using System;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using static SightsNavigator.Models.City;


namespace SightsNavigator.Services.SightService
{
    internal class SightRequest : ISightRequest
    {
        private static System.Timers.Timer RequesTimer;
        private static int _timerPeriod = 1000;
        public SightRequest()
        {
            RequesTimer = new System.Timers.Timer(_timerPeriod);
            RequesTimer.Elapsed += async (sender, e) => await HandleTimer();

        }


        /// <summary>
        /// Method tries to find a city according the passed information
        /// </summary>
        /// <returns>Returns an object of the City class that keeps basic information and list of Xids</returns>
        public async Task<City> GetCityAsync(string name, string country = "auto")
        {
            //Step 1 -- basic information of the city
            //Get default city information 
            City city = await GetBasicCityInfoAsync(name, country);
            //Step 2 -- list of Xids
            if (city is null) return null;
            //Get list of Xids of the city 
            var radius = 30_000; // HARDCODED
            var xids = await GetSightsListOfCityAsync(city, radius);
            //insert the list inside of city object
            if (xids is null) return null;
            city.ListOfXids = xids.ToList<String>();
            return city;
        }



        /// <summary>
        /// Method find the basic information about the city like lon, lat, timezone and so on.
        /// </summary>
        /// <returns>returns an object of the City class which contains basic information.</returns>
        /// <remarks>Remark: It DOESN'T contain a list of Xids</remarks>
        public async Task<City> GetBasicCityInfoAsync(string name, string country = "auto")
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city">An object of the City class</param>
        /// <param name="radius">Radius in meters</param>
        /// <returns></returns>
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

        /// <summary>
        /// Method tries to fetch a new list of  sigths according to the list of Xids
        /// </summary>
        /// <param name="Xids">The list of sights' Ids.</param>
        /// <returns>A list of City.Sight objects
        /// </returns>
        /// <remarks>Remarks: 
        /// Method must be restricted outside of the call if the API has limiation
        /// </remarks>
        public async Task<IEnumerable<City.Sight>> GetChunckOfSights(List<String> Xids)
        {   
            if(Xids  == null) { return null; }
            if (!Xids.Any()) { return null; }

            List<City.Sight> sights = new List<City.Sight>();
            WebRequest web = new WebRequest();
            HttpResponseMessage response;

            foreach (var xid in Xids)
            {
                if(String.IsNullOrEmpty(xid)) continue;
                string url = GetSightURL(xid);
                if (url == "not correct link") continue;
                response = await web.FetchDataFromAPI(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var converter = JsonConvert.DeserializeObject<dynamic>(json);
                    var sight = ParseTheSight(converter);
                    if(sight is not null)
                    {
                        //if(String.Equals(sight.Image, _errorImageDecode))
                        sights.Add(sight);
                    }
                }
            }

            return sights.ToList(); 
        }

        /// <summary>
        /// Method parses the object to output of the Sight class
        /// </summary>
        /// <returns>Returns: The sight object or null</returns>

        private City.Sight ParseTheSight(dynamic converter)
        {
            City.Sight sight = new City.Sight();
            
            //Name
            string name = ParseSightName(converter);
            sight.Name = name;

            //Xid
            string xid = ParseSightXid(converter);
            sight.Xid = xid;

            //Lat/Lon
            double lat = converter.point.lat;
            double lon = converter.point.lon;
            sight.Lat = lat;
            sight.Lon = lon;

            //Address
            sight.SightAddress = ParseSightAddress(converter);

            //Wikipedia_extracts 
            var wikipedia_extracts = converter.wikipedia_extracts;
            if (wikipedia_extracts is not null)
            {
                var description = TryParseJsonLine(wikipedia_extracts.text);
                sight.Description = description;
            }
            
            //Image
            string image = ParseSightImage(converter);
            sight.Image = image;

            return sight;
        }

        //-------------ERROR-HANDLINGs-START---------------//
        public string ErrorNameDecode { get => _errorNameDecode;}
        private string _errorNameDecode = "ERROR_DECODE_NAME";
        private string ParseSightName(dynamic converter)
        {
            return converter.name ?? _errorNameDecode;
        }


        public string ErrorXidDecode { get => _errorXidDecode; }
        private string _errorXidDecode = "ERROR_DECODE_XID";
        private string ParseSightXid(dynamic converter)
        {
            return converter.xid ?? _errorXidDecode;
        }


        private City.Sight.Address ParseSightAddress(dynamic converter)
        {
            if (converter.address == null)
                return null;
            var Address = converter.address;
            string _city = TryParseJsonLine(Address.city);
            var _road = TryParseJsonLine(Address.road);
            var _state = TryParseJsonLine(Address.state);
            var _country = TryParseJsonLine(Address.country);
            var _suburb = TryParseJsonLine(Address.subsurb);
            var _pedestrian = TryParseJsonLine(Address.pedestrian);
            var _zip = TryParseJsonLine(Address.postcode);
            var _house_number = TryParseJsonLine(Address.house_number);
            var _state_district = TryParseJsonLine(Address.state_district);

            var address = new City.Sight.Address(
                city: _city,
                country: _country,
                state: _state,
                subUrb: _suburb,
                pedestrian: _pedestrian
                );
            return address;
        }

        public string ErrorImageDecode { get => _errorImageDecode; }
        private string _errorImageDecode = "ERROR_DECODE_IMAGE";
        private string ParseSightImage(dynamic converter)
        {
            string image = _errorImageDecode; 
            var preview = converter.preview;
            if (preview is not null)
            {
                var source = TryParseJsonLine(preview.source);
                image = source;
            }
            return image;
        }

        //-------------ERROR-HANDLINGs-END---------------//





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
            _timerPeriod = (int)newPeriod;
            RequesTimer.Interval = newPeriod;
        }

        public string TryParseJsonLine(object line) {
            if (line == null) return "";
            var str = line.ToString();
            return !String.IsNullOrEmpty(str) ? str.ToString() : "";
        }
            
    }


    //TRASH SECTION
    //private async Task<IEnumerable<City.Sight>> GetListOfSights(City city)
    //{
    //    /*DOESNT FULLY WORK*/
    //    if (city is null) return null;
    //    if (city.ListOfXids is null) return null;
    //    if (city.ListOfXids.Count() == 0) return null;

    //    var xids = city.ListOfXids; ;
    //    string url;

    //    //RequesTimer.Start();

    //    int start = 0;
    //    int step = 5;
    //    int end = xids.Count();

    //    string xid;

    //    WebRequest web = new WebRequest();
    //    HttpResponseMessage response;

    //    City.Sight blankSight = new City.Sight();

    //    var tasks = new List<Task<HttpResponseMessage>>();
    //    var badtasks = new List<Task<HttpResponseMessage>>();

    //    var responses = new List<HttpResponseMessage>();

    //    var sights = new List<City.Sight>();
    //    int endchunk;

    //    System.Diagnostics.Stopwatch localwatch = new System.Diagnostics.Stopwatch();
    //    DateTime beginmeasure;
    //    watch.Start();
    //    bool success = true;

    //    while (start < end)
    //    {
    //        try
    //        {
    //            if (end - start < step) step = end - start;
    //            else step = 10;

    //            endchunk = (step + start > end) ? end : step + start;

    //            System.Diagnostics.Debug.WriteLine($"[{start} ... {endchunk}]");

    //            //Tasks filling
    //            beginmeasure = DateTime.UtcNow;
    //            for (int i = start; i < endchunk; i++)
    //            {
    //                xid = xids[i];
    //                url = GetSightURL(xid);
    //                tasks.Add(web.FetchDataFromAPI(url));
    //            }
    //            System.Diagnostics.Debug.WriteLine($"Execution Time -- fill tasks: {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");

    //            /*ATTENTION!
    //             BIG MEMORY LEAKAGE!
    //              1 check the type of tasks 
    //              2 response don't have to exist
    //              3 check type in functions AllTasksSuccess and RetryUnSuccessTasks
    //             */


    //            //WhenAll task will be completed
    //            beginmeasure = DateTime.UtcNow;
    //            await Task.WhenAll(tasks.ToArray());
    //            System.Diagnostics.Debug.WriteLine($"Execution Time -- execution for all tasks: {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");


    //            //Bad tasks
    //            beginmeasure = DateTime.UtcNow;
    //            badtasks = tasks.Where(task => !task.Result.IsSuccessStatusCode).ToList();
    //            tasks.RemoveAll(task => !task.Result.IsSuccessStatusCode);
    //            if (tasks.Count() < step)
    //            {
    //                var b = 3 + 3;
    //            }

    //            System.Diagnostics.Debug.WriteLine($"Execution Time -- bad task filtering: {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");

    //            while (badtasks.Count() > 0)
    //            {
    //                var completed = await Task.WhenAny(badtasks.ToArray());
    //                if (completed.Result.IsSuccessStatusCode)
    //                {
    //                    tasks.Add(completed);
    //                    badtasks.Remove(completed);
    //                }
    //                else
    //                {
    //                    await Task.Delay(_timerPeriod);
    //                }
    //            }



    //            //Filling responses
    //            beginmeasure = DateTime.UtcNow;
    //            tasks.ForEach(task =>
    //            {
    //                responses.Add(task.Result);
    //            });
    //            System.Diagnostics.Debug.WriteLine($"Execution Time -- fill responses: {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");

    //            //Getting sightsObjects
    //            beginmeasure = DateTime.UtcNow;
    //            var sightsObjects = await DecodeSight(responses);
    //            System.Diagnostics.Debug.WriteLine($"Execution Time -- decoding sights: {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");

    //            //Filtering and filling sights
    //            beginmeasure = DateTime.UtcNow;
    //            sightsObjects.ForEach(sight =>
    //            {
    //                if (sight != null)
    //                {
    //                    if (!sights.Contains(sight)) sights.Add(sight);

    //                }
    //            });
    //            System.Diagnostics.Debug.WriteLine($"Execution Time -- filtering sights: {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");

    //            //await Task.Delay((int)_timerPeriod);

    //            //beginmeasure = DateTime.UtcNow;
    //            //await Task.Delay((int)_timerPeriod*2);
    //            //System.Diagnostics.Debug.WriteLine($"Execution Time -- delay {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");

    //            //Delay
    //            beginmeasure = DateTime.UtcNow;
    //            await Task.Delay(_timerPeriod);
    //            System.Diagnostics.Debug.WriteLine($"Execution Time -- DELAY REQUEST: {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");


    //            beginmeasure = DateTime.UtcNow;
    //            if (start + step > end) break;
    //            else start += step;
    //            tasks.Clear();
    //            badtasks.Clear();
    //            responses.Clear();
    //            System.Diagnostics.Debug.WriteLine($"Execution Time -- range changing and clearance: {(beginmeasure - DateTime.UtcNow).ToString("mm\\:ss\\.ff")}");

    //            //Thread.Sleep(1500);
    //            //if (start + step > end) break;
    //            //else start += step;
    //            //tasks.Clear();
    //            //responses.Clear();

    //            System.Diagnostics.Debug.WriteLine("---------------------------------\n");

    //        }
    //        catch (Exception ex)
    //        {
    //            System.Diagnostics.Debug.WriteLine(ex);
    //        }

    //    }
    //    watch.Stop();
    //    System.Diagnostics.Debug.WriteLine($"Execution Time: {watch.Elapsed} s");
    //    return sights;
    //}



    //private async Task<IEnumerable<City.Sight>> GetFirstTenSights(City city)
    //{
    //    if (city is null) return null;
    //    if (city.ListOfXids is null) return null;
    //    if (city.ListOfXids.Count() == 0) return null;

    //    var xids = city.ListOfXids; ;
    //    string url, xid;

    //    var tasks = new List<Task<HttpResponseMessage>>();
    //    var sights = new List<City.Sight>();

    //    WebRequest web = new WebRequest();
    //    HttpResponseMessage response;

    //    for (int i = 0; i < 10; i++)
    //    {
    //        xid = xids[i];
    //        url = GetSightURL(xid);
    //        tasks.Add(web.FetchDataFromAPI(url));
    //    }
    //    await Task.WhenAll(tasks.ToArray());


    //    for (int i = 0; i < 10; i++)
    //    {
    //        if (tasks[i].Result.IsSuccessStatusCode)
    //        {
    //            var sight = new City.Sight();
    //            var json = await tasks[i].Result.Content.ReadAsStringAsync();
    //            var converter = JsonConvert.DeserializeObject<dynamic>(json);



    //            sight.Name = converter.name;
    //            sight.Xid = converter.xid;


    //            if (converter.address is not null)
    //            {
    //                var Address = converter.address;
    //                string _city = TryParseJsonLine(Address.city);
    //                var _road = TryParseJsonLine(Address.road);



    //                var _state = TryParseJsonLine(Address.state);
    //                var _country = TryParseJsonLine(Address.country);
    //                var _suburb = TryParseJsonLine(Address.subsurb);
    //                var _pedestrian = TryParseJsonLine(Address.pedestrian);
    //                var _zip = TryParseJsonLine(Address.postcode);
    //                var _house_number = TryParseJsonLine(Address.house_number);
    //                var _state_district = TryParseJsonLine(Address.state_district);

    //                sight.SightAddress = new City.Sight.Address(
    //                    city: _city,
    //                    country: _country,
    //                    state: _state,
    //                    subUrb: _suburb,
    //                    pedestrian: _pedestrian
    //                    );
    //            }


    //            var wikipedia_extracts = converter.wikipedia_extracts;
    //            if (wikipedia_extracts is not null)
    //            {
    //                var description = TryParseJsonLine(wikipedia_extracts.text);
    //                sight.Description = description;
    //            }



    //            //var image = TryParseJsonLine(converter.image);
    //            var preview = converter.preview;
    //            if (preview is not null)
    //            {
    //                var source = TryParseJsonLine(preview.source);
    //                var image = source;
    //                sight.Image = image;
    //            }



    //            //sight.Image = image;
    //            sights.Add(sight);
    //        }
    //    }
    //    return sights.ToList();
    //}


}
