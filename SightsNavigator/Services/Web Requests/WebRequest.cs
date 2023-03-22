using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Services
{
    internal class WebRequest : IWebRequest
    {
        public async Task<HttpResponseMessage> FetchDataFromAPI(string url)
        {
            try
            {
                var client = new HttpClient();
                HttpResponseMessage message = await client.GetAsync(url);
                return message;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
