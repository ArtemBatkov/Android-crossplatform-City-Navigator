using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Models
{
    public class City: CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
       

        //public fields
        public string DefaultBackground { get => _defaultbg; }
        private static string _defaultbg = "trip_blank.png";


        public string CurrentBackground
        {
            get => _currentbackground;
            set {
                if(_currentbackground != value)
                {
                    _currentbackground = value;
                    OnPropertyChanged(nameof(CurrentBackground));
                }                
            }
        }
        private string _currentbackground = _defaultbg;
        
        public List<string> Backgrounds
        {
            get
            {
                return _backgrounds;
            }
            set
            {
                if(value.Count > 0)
                {
                    _backgrounds = value;
                    if (_currentbackground.Equals(_defaultbg))
                    {
                        _currentbackground = _backgrounds.Last();
                    }
                }
            }
        }
        private List<string> _backgrounds;


        public List<Sight> FavouriteSights = new List<Sight>();


        public string Name { get => _name; set => _name = value; }
        public string Country { get => _country; set => _country = value; }
        public double Lat { get => _latitude; set => _latitude = value; }
        public double Lon { get => _longitude; set => _longitude = value; }
        public long Population { get => _population; set => _population = value; }
        public string Timezone { get => _timezone; set => _timezone = value; }
        public string Status { get => _status; set => _status = value; }

        //private fields
        private string _name;
        private string _country;
        private double _latitude;
        private double _longitude;
        private long _population;
        private string _timezone;
        private string _status;



        //List of Xids
        public List<string> ListOfXids { get => _xidList; set => _xidList = value; }
        private List<String> _xidList;

        public List<Sight> SightList = new List<Sight>();

  

        public class Sight
        {
            public string Name { get => _name; set => _name = value; }
            public string Xid { get => _xid; set => _xid = value; }
            public double Lat { get => _lat; set => _lat = value; }
            public double Lon { get => _lon; set => _lon = value; }

            public string Image { get => _image; set => _image = value; }

            



            public string Description { get => _description; set => _description = value; }

            private string _xid;
            private string _name;
            private double _lat;
            private double _lon;
            private string _image;
            private string _description;
            private Address _addr;

           
            public Address SightAddress { get => _addr; set => _addr = value; }

            public class Address
            {

                public Address(string city = "", string country = "", string state = "", string subUrb = "", string pedestrian = "", 
                    string stateDistrict = "")
                {
                    _city = city;
                    _state = state;
                    _suburb = subUrb;
                    _stateDistrict = stateDistrict;                                     
                    _pedestrian = pedestrian;

                    _address = $"city: {_city}\n" +
                            $"state: {_state}\n" +
                            $"country: {_country}\n" +
                            $"subub: {_suburb}\n" +
                            $"pedestrian: {_pedestrian}\n" +
                            $"disctrict: {_stateDistrict}";
                }


                public string FullAddress { get => _address;}

                public string City { get => _city; set => _city = value; }
                public string State { get => _state; set => _state = value; }
                public string SubUrb { get => _suburb; set => _suburb = value; }
                public string Pedestrian { get => _pedestrian; set => _pedestrian = value; }
                public string StateDistrict { get => _stateDistrict; set => _stateDistrict = value; }
                public string Country { get => _country; set=>_country= value; }



                private string _address;
                private string _city;
                private string _state;
                private string _suburb;
                private string _pedestrian;
                private string _stateDistrict;
                private string _country;

                
            }


        }




    }
}
