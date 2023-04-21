
using MvvmHelpers;
using MvvmHelpers.Commands;
using SightsNavigator.Models;
using SightsNavigator.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SightsNavigator.ViewModels
{
    public class TripDetailedViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public String zalupaNaVorotnike { get; } = "blank_sight_2.jpg";



        public City city { get => _city;
            set
            { 
               _city = value;
               OnPropertyChanged(nameof(_city));
               
            }
        }

        private City _city; 

        public UriImageSource BackgroundImage { get => _backgroundImage; 
            set {  
                _backgroundImage = value; 
                OnPropertyChanged(nameof(BackgroundImage));
            }
        }
        private UriImageSource _backgroundImage; 

        



        public ObservableRangeCollection<City.Sight> Favourites { get; set; }
        public INavigation navigation;


        public City.Sight FavouriteSelected { get => _favouriteSelected; 
            set
            {
                _favouriteSelected = value;
                OnPropertyChanged(nameof(FavouriteSelected));
            }
                
        }
        private City.Sight _favouriteSelected;

        public IServiceProvider _serviceProvider;

        private TripEditViewModel _tripEditViewModel { get; set; }



        public TripDetailedViewModel(City city, TripEditViewModel tripEditViewModel, IServiceProvider serviceProvider)
        {
            this._tripEditViewModel = tripEditViewModel;
            this._city = city;
            var uri = new Uri(city.CurrentBackground);
            var urimage = new UriImageSource();
            urimage.Uri = uri;
            BackgroundImage = urimage;

            PageAppearingCommand = new AsyncCommand(PageAppearing);
            SelectedItemCommand = new AsyncCommand(onSelectedItem);
            Favourites = new ObservableRangeCollection<City.Sight>();
            onTripEditPage = new AsyncCommand(gotoEditPage);
            var favourites = city.FavouriteSights;
            favourites.ForEach(fav => Favourites.Insert(0, fav));
            _serviceProvider = serviceProvider;

            //tripEditViewModel = new TripEditViewModel(city);
            //tripEditViewModel.CityChanged += OnCityChanged;
        }

        private void OnCityChanged(object sender, City e)
        {
            var city = e as City; 

        }


        //S------COMMANDS-----//
        public AsyncCommand SelectedItemCommand { get; set; }
        public AsyncCommand PageAppearingCommand { get; set; }
        public AsyncCommand onTripEditPage { get; set; }
        //E------COMMANDS-----//

        private async Task PageAppearing()
        {
            await Refresh();
        }


        public async Task Refresh()
        {
            var f = 3;
           
        }


        private async Task gotoEditPage()
        {
            await navigation.PushAsync(new TripEditPage(city, _tripEditViewModel));
        }



        private async Task onSelectedItem()
        {
            if (_favouriteSelected == null) return;
            await navigation.PushAsync(new DetailedPage(city, _favouriteSelected, _serviceProvider ));
            FavouriteSelected = null;
            return;
        }
    }
}
