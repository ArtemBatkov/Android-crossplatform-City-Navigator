using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Services.GooglePlaceService
{
    internal class GooglePlaceService: IGooglePlaceService
    {
        public async Task<string> GetGooglePlaceIdAsync(string name)
        {
            var url = $"https://maps.googleapis.com/maps/api/place/findplacefromtext/json?input={name}" +
                      $"&inputtype=textquery&key={_googleKey}";
            WebRequest web = new WebRequest();
            HttpResponseMessage response = await web.FetchDataFromAPI(url);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var converter = JsonConvert.DeserializeObject<dynamic>(json);
                var status = converter.status;


                if (status == "ZERO_RESULTS") return "";
                if (converter.candidates[0] == null)
                    return "";
                var place_id = converter.candidates[0].place_id.ToString();
                return place_id;

            }
            else return PLACE_ID_ERROR;
        }
        public readonly string PLACE_ID_ERROR = "NO PLACE ID";
        private readonly string _googleKey = "AIzaSyDX9V4YhTFseL8gwoeWTszaLzf8y_Nw-nI";
    }
}
