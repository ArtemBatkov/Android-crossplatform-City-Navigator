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

namespace SightsNavigator.ViewModels
{
    class SearchCitySightsViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public ISightRequest service => DependencyService.Get<ISightRequest>();
        public ObservableRangeCollection <City.Sight> Sights { get; set; }
        public SearchCitySightsViewModel() {
            PageAppearingCommand = new AsyncCommand(PageAppearing);
            SearchSightsCommand = new Command(onSearchSights);
            Sights = new ObservableRangeCollection<City.Sight>();
        }

        // COMMANDS - start
        public AsyncCommand PageAppearingCommand { get; set; }
        public ICommand SearchSightsCommand { get; set; }
        // COMMANDS - end




        //FUNCTIONS - start
        private async void  onSearchSights()
        {
            City city = await service.GetCityAsync("Moscow");
            if(city.SightList is not null)
            {
                if (city.SightList.Count == 0) return;
                Sights.Clear();
                for(int i = 0; i < city.SightList.Count(); i++)
                {
                    Sights.Insert(0, city.SightList[i]);
                }
                
            }
        }






        private async Task PageAppearing()
        {
            await Refresh();
        }

        public async Task Refresh()
        {

        }
        //FUNCTIONS - end



    }
}
