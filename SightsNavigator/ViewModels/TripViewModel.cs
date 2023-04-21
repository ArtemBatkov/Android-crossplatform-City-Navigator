using MvvmHelpers;
using MvvmHelpers.Commands;
using SightsNavigator.Models;
using SightsNavigator.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Command = MvvmHelpers.Commands.Command;

namespace SightsNavigator.ViewModels
{
    public class TripViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {

        public TripEditViewModel _tripEditViewModel;


        public INavigation navigation { get; set; }
        public ObservableRangeCollection<City> Trips { get ; set; }

        public IServiceProvider _serviceProvider;

        public TripViewModel(IServiceProvider serviceProvider) {
            this._serviceProvider = serviceProvider;
            PageAppearingCommand = new AsyncCommand(PageAppearing);
            Trips = new ObservableRangeCollection<City>();
            SelectedItemCommand = new AsyncCommand(onSelectedTrip);
            var trips = TripListModel.GetTripList();
            trips.ForEach(trip => Trips.Insert(0, trip));            
        }

      


        //S-----COMMANDS-----//
        public AsyncCommand PageAppearingCommand { get; set; }
        public AsyncCommand SelectedItemCommand { get; set; }
        //E-----COMMANDS-----//

        public City TripSelected { get => _tripselected;
            set
            {    
                _tripselected = value;
                OnPropertyChanged(nameof(TripSelected));                
            }
        }
        private City _tripselected = null;

        public String Avatar { get; set; }
        private string _avatar;
        
        public String Title { get; set; }
        private string _title;

        private async Task onSelectedTrip()
        {
            if (_tripselected == null) return;
            _tripEditViewModel = new TripEditViewModel(_tripselected);
            _tripEditViewModel.CityChanged += OnCityChanged;
            await navigation.PushAsync(new TripDetailedPage(_tripselected, _tripEditViewModel, _serviceProvider));
            TripSelected = null;
            return;
        }

        private void OnCityChanged(object sender, City e)
        {
            TripSelected = e;
            OnPropertyChanged(nameof(TripSelected));
        }



        private async Task PageAppearing()
        { 
            await Refresh();
        }


        public async Task Refresh()
        {   
           
           
        }
    }
}
