
using Newtonsoft.Json;
using SightsNavigator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Services.GooglePlaceService
{
    public class GooglePlaceService : IGooglePlaceService
    {
        private readonly string gkey = Environment.GetEnvironmentVariable("GoogleMapsApiKey");
        private GooglePlace _google;
        public readonly string ErrorId = "ERROR_ID";

        private string _gkey;

        public GooglePlaceService(string gkey)
        {
            this.gkey = gkey;
        }
        public async Task<GooglePlace> getDataFromGoogle(GooglePlace google)
        {
            this._google = google;
            var f = Environment.GetEnvironmentVariable("GoogleMapsApiKey");
            string id = await getGooglePlaceId();
            if(String.IsNullOrEmpty(id))
            {
                return null;
            }
            _google.GooglePlaceCityId = id;
            var references = await getPhotoPlaceReferences();
            _google.GooglePlacePhotoReferences = references;             
            var photos = await getPhotoPlaceCandidates();
            _google.GooglePlacePhotoCandidates = photos;
            return _google;
        }   
        
        private async Task<string> getGooglePlaceId()
        {
            if(_google == null) { return ErrorId; }
            string name = _google.City.Name.ToLower();
            string reg = _google.City.Country.ToLower();           
            var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={name}&types=(cities)&language=en&components=country:{reg}&key={gkey}";
            WebRequest web = new WebRequest();
            HttpResponseMessage response = await web.FetchDataFromAPI(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var converter = JsonConvert.DeserializeObject<dynamic>(json);
                string status = converter.status;
                if(status.Equals("OK"))
                {
                    var predictions = converter.predictions;
                    var pids = new List<String>();//posible ids
                    //fill possible ids
                    foreach ( var prediction in predictions )
                    {
                        string pid = prediction.place_id;
                        if (!String.IsNullOrEmpty(pid))
                        {
                            pids.Insert(0, pid);
                        }
                    }
                    if(pids.Count == 0)
                    {
                        //zero items
                        return ErrorId;
                    }
                    else if(pids.Count == 1)
                    {
                        return pids[0];
                    }
                    else
                    {
                        //more than 1 item
                        string trueId = await filterIds(pids);
                        return trueId;
                    }
                }
                else
                {//not OK status code
                    return ErrorId;
                }
            }
            else
            {//bad response
                return ErrorId;
            }
        }


        private async Task<string> filterIds(List<string> ids)
        {
            if (_google == null) return ErrorId;            
            string name = _google.City.Name;
            double lat = _google.City.Lat;
            double lon = _google.City.Lon;
            WebRequest web = new WebRequest();
            HttpResponseMessage response; 
            foreach (var id in ids)
            {
                var url = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={id}&fields=name,geometry&key={gkey}";
                response =  await web.FetchDataFromAPI(url);
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var converter = JsonConvert.DeserializeObject<dynamic>(json);
                    string status = converter.status;
                    if (status.Equals("OK"))
                    {
                        var result = converter.result;
                        var geometry = result.geometry;
                        var viewport = geometry.viewport;
                        var northeast = viewport.northeast;
                        var southwest = viewport.southwest;
                        var nlon = (double) northeast.lng;
                        var nlat = (double)northeast.lat;
                        var slon = (double) southwest.lng;
                        var slat = (double) southwest.lat;

                        var latrange = (lat >= slat) && (lat <= nlat);
                        var lonrange = (lon >= slon) && (lon <= nlon);
                        var range = latrange && lonrange;

                        if (range)                       
                            return id;                        
                    }
                    else return ErrorId;
                }
                else
                {
                    return ErrorId;
                }
            }
            return ErrorId;
        }

        private async Task<List<string>> getPhotoPlaceReferences()
        {
            var photos = new List<string>();
            var id = _google.GooglePlaceCityId;
            var url = $"https://maps.googleapis.com/maps/api/place/details/json?place_id={id}&fields=photo&key={gkey}";
            WebRequest web = new WebRequest();
            HttpResponseMessage response = await web.FetchDataFromAPI(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var converter = JsonConvert.DeserializeObject<dynamic>(json);
                string status = converter.status;
                if (status.Equals("OK"))
                {
                    var result = converter.result;
                    var photosCandidates = result.photos;
                    foreach( var photo in photosCandidates)
                    {
                        string reference = photo.photo_reference;
                        if (!String.IsNullOrWhiteSpace(reference))
                        {
                            photos.Insert(0, reference);
                        }
                    }                    
                }               
            }
            return photos;
        }


        private async Task<List<string>> getPhotoPlaceCandidates()
        {
            var photos = new List<string>();
            var references = _google.GooglePlacePhotoReferences;
            foreach(var reference in references)
            {
                var url = $"https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photoreference={reference}&sensor=false&key={gkey}";
                photos.Insert(0, url); 
            }
            return photos;
        }
    }
}
