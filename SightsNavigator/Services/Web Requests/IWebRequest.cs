using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Services
{
    internal interface IWebRequest
    {
        public Task<HttpResponseMessage> FetchDataFromAPI(string url);

   

    }
}
