using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SightsNavigator.Models;
using SightsNavigator.Services;


using System.Globalization;
using System.Windows.Input;
using Command = MvvmHelpers.Commands.Command;
using CommunityToolkit.Mvvm.ComponentModel;
using MvvmHelpers.Commands;
using SightsNavigator.Services.SightService;
using System.Collections.ObjectModel;
using MvvmHelpers;
using Microsoft.Maui.Layouts;

namespace SightsNavigator.ViewModels
{
    class SearchCitySightsViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public ISightRequest service => DependencyService.Get<ISightRequest>();
        public IWebRequest webservice => DependencyService.Get<IWebRequest>();
        public ObservableRangeCollection <City.Sight> Sights { get; set; }
        public SearchCitySightsViewModel() {
            PageAppearingCommand = new AsyncCommand(PageAppearing);
            SearchSightsCommand = new Command(onSearchSights);
            Sights = new ObservableRangeCollection<City.Sight>();
            FooterVisible = false;
        }

  

        // COMMANDS - start
        public AsyncCommand PageAppearingCommand { get; set; }
        public ICommand SearchSightsCommand { get; set; }
        
        // COMMANDS - end


        //Properties - start
        public bool FooterVisible {
            get => _footerVisible;
            set {
                if(_footerVisible != value)
                   _footerVisible = value;
                OnPropertyChanged(nameof(FooterVisible));

            }
        }
        
        private bool _footerVisible = false;
        //Properties - end



        //FUNCTIONS - start
        private async void onSearchSights()
        {
            City city = await service.GetCityAsync("Moscow");
            if (city.SightList is not null)
            {
                if (city.SightList.Count == 0) return;
                Sights.Clear();
                for (int i = 0; i < city.SightList.Count(); i++)
                {
                    Sights.Insert(0, city.SightList[i]);
                }
            }

            if (city is not null)
            {
                if (city.ListOfXids is not null || city.ListOfXids.Count() != 0)
                {
                    var GoogleIds = new List<string>();
                    var sights = city.SightList.ToList();

                    for (int i = 0; i < sights.Count(); i++)
                    {
                        var xid = sights[i].Xid.ToString();
                        var name = sights[i].Name.ToString();
                        var address = sights[i].SightAddress;
                        var suburb = address.SubUrb;
                        var pedestrians = address.Pedestrian;
                        //string place_id = await webservice.GetGooglePlaceIdAsync(
                        //    String.IsNullOrEmpty(name) ? pedestrians : name
                        //    );
                    }
                }
            }
        }

          
        public void onSightsScrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            //page injection
            var collectionView = sender as CollectionView;
            var items = collectionView.ItemsSource as IList<City.Sight>; 
            if (items == null || items.Count == 0 )
            {
                FooterVisible = false;
                return;
            }
            var lastVisibleItemIndex = e.LastVisibleItemIndex;
            var lastItemIndex = items.Count - 1;

            if(lastItemIndex == lastVisibleItemIndex)
            {
                FooterVisible = true;

            }
            else
            {
                FooterVisible = false;
            }

            
            
            System.Diagnostics.Debug.WriteLine("Showing footer");
        }

       

        private async Task PageAppearing()
        {
            FooterVisible = false;
            await Refresh();
        }

        public async Task Refresh()
        {
            System.Diagnostics.Debug.WriteLine($"Visible: {_footerVisible}");
        }
        //FUNCTIONS - end



    }
}
