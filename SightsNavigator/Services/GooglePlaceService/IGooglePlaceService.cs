using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Services.GooglePlaceService
{
    internal interface IGooglePlaceService
    {
        public Task<String> GetGooglePlaceIdAsync(string name);
    }
}
