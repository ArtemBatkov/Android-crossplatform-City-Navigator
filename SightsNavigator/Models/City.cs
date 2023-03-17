using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.Models
{
    internal class City
    {
        //public fields
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

        public List<Sight> SightList { get; set; }
        

        public class Sight
        {
            public string Name { get => _name; set => _name = value; }
            public string Xid { get => _xid; set => _xid = value; }

            private string _xid;
            private string _name;
        }



    }
}
