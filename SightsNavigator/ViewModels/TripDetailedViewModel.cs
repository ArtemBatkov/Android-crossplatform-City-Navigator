
using MvvmHelpers;
using MvvmHelpers.Commands;
using SightsNavigator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.ViewModels
{
    public class TripDetailedViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public City city { get; set; }

        public string BackgroundImage {get => _backgroundImage; set => _backgroundImage = value; }
        private string _backgroundImage; 
        public ObservableRangeCollection<City.Sight> Favourites { get; set; }
        public INavigation navigation;


        public City.Sight FavouriteSelected { get; set; }
        private City.Sight _favouriteSelected;

        public TripDetailedViewModel() {
            PageAppearingCommand = new AsyncCommand(PageAppearing);
            SelectedItemCommand = new AsyncCommand(onSelectedItem);
            Favourites = new ObservableRangeCollection<City.Sight>();
            
        }

       

        //S------COMMANDS-----//
        public AsyncCommand SelectedItemCommand { get; set; }
        public AsyncCommand PageAppearingCommand { get; set; }
        //E------COMMANDS-----//

        private async Task PageAppearing()
        {
            await Refresh();
        }


        public async Task Refresh()
        {
            var favourites = city.FavouriteSights;
            favourites.ForEach(fav => Favourites.Insert(0, fav));
           
        }




        private async Task onSelectedItem()
        {
            return;
        }
    }
}
