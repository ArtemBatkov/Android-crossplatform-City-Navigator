using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Models
{
    public class GooglePlace
    {
        public string GooglePlaceCityId { get => _googlePlaceCityId; set => _googlePlaceCityId = value; }
        private string _googlePlaceCityId;

        public List<string> GooglePlacePhotoCandidates { get => _googlePlacePhotoCandidates; set => _googlePlacePhotoCandidates = value; }
        private List<string> _googlePlacePhotoCandidates; 

        public List<string> GooglePlacePhotoReferences { get => _googlePlacePhotoReference; set => _googlePlacePhotoReference = value; }
        private List<string> _googlePlacePhotoReference;


        public City City { get => _city; }
        private City _city;
        
        public GooglePlace(City city) { 
            this._city = city;
        }
    }
}
