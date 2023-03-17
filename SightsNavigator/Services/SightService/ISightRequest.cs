using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SightsNavigator.Models;
namespace SightsNavigator.Services.SightService
{
    internal interface ISightRequest
    {
        public Task<City> GetCityAsync(string name, string country = "auto");

        public Task<IEnumerable<String>> GetSightsListOfCityAsync (City city, double radius);
    }
}
