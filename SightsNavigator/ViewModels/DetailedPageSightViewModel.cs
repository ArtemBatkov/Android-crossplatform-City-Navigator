using MvvmHelpers.Commands;
using SightsNavigator.Models;

using Microsoft.Maui.Controls.Maps;
using System.Diagnostics;

namespace SightsNavigator.ViewModels
{

    class DetailedPageSightViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public DetailedPageSightViewModel() { }

        private readonly City.Sight sight;
        public DetailedPageSightViewModel(City.Sight sight)
        {
            this.sight = sight;
            PageAppearingCommand = new AsyncCommand(PageAppearing);
            _dName = sight.Name;
            _dxid = sight.Xid;
            _daddress = sight.SightAddress.FullAddress;
            _dimage = sight.Image;
            _ddescription = sight.Description;
            _dlat = sight.Lat;
            _dlon = sight.Lon;
            try
            {
                DLocation = new Location(_dlat, _dlon);
            }
            catch (Exception ex)
            {
                Debugger.Log(0, "Error", ex.ToString());
            }
            WhatIsVisible();
        }


        // COMMANDS - start
        public AsyncCommand PageAppearingCommand { get; set; }
        // COMMANDS - end



        public String DName { get => _dName; set => _dName = value; }
        private string _dName;
        public bool HasName { get => _hasname; set => _hasname = value; }
        private bool _hasname;

        public String DXid { get => _dxid; set => _dxid = value; }
        private string _dxid;

        public String DAddress { get => _daddress; set => _daddress = value; }
        private string _daddress;
        public bool HasAddress { get => _hasaddress; set => _hasaddress = value; }
        private bool _hasaddress;

        public String DImage { get => _dimage; set => _dimage = value; }
        private string _dimage;

        public String DDescription { get => _ddescription; set => _ddescription = value; }
        private string _ddescription;

        public double DLat { get => _dlat; set => _dlat = value; }
        public double DLon { get => _dlon; set => _dlon = value; }
        private double _dlat;
        private double _dlon;

        public Location DLocation { get; set; }
        private void WhatIsVisible()
        {
            if (!String.IsNullOrEmpty(sight.Name))
            {
                HasName = true;
            }
            else
            {
                HasName = false;
            }
            HasAddress = !HasName;
        }


        private async Task PageAppearing()
        {
            await Refresh();
        }

        public async Task Refresh()
        {
            var b = 3;
        }
    }
}
